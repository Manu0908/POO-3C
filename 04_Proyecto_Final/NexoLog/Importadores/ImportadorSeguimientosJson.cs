using System.Text;
using System.Text.Json;
using NexoLog.DTOs;

namespace NexoLog.Importadores;

public class ImportadorSeguimientosJson : IImportadorArchivo<Acontecimiento>
{
    public List<Acontecimiento> Importar(string rutaArchivo, List<RegistroInconsistencia> inconsistencias)
    {
        var acontecimientosValidos = new List<Acontecimiento>();

        try
        {
            string contenidoJson = File.ReadAllText(rutaArchivo, Encoding.UTF8);
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var eventos = JsonSerializer.Deserialize<List<AcontecimientoDto>>(contenidoJson, opciones);
            if (eventos == null)
            {
                eventos = new List<AcontecimientoDto>();
            }

            foreach (var evtDto in eventos)
            {
                try
                {
                    acontecimientosValidos.Add(Acontecimiento.MapToDomain(evtDto));
                }
                catch (NexoLogException ex)
                {
                    string refEvt = "EVENTO_INCOMPLETO";
                    if (!string.IsNullOrWhiteSpace(evtDto.IdAcontecimiento))
                    {
                        refEvt = evtDto.IdAcontecimiento;
                    }
                    inconsistencias.Add(new RegistroInconsistencia(rutaArchivo, refEvt, RegistroInconsistencia.TipoDatoInvalido, ex.Message, DateTime.Now));
                }
            }
        }
        catch (Exception ex)
        {
            inconsistencias.Add(new RegistroInconsistencia(rutaArchivo, "GENERAL", RegistroInconsistencia.TipoDatoInvalido, ex.Message, DateTime.Now));
        }

        return acontecimientosValidos;
    }
}
