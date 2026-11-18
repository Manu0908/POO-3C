using System.Text;
using System.Text.Json;
using NexoLog.DTOs;

namespace NexoLog.Importadores;

public class ImportadorSeguimientosJson : IImportadorArchivo<Acontecimiento>
{
    public List<Acontecimiento> Importar(string rutaArchivo, List<RegistroInconsistencia> inconsistencias)
    {
        List<Acontecimiento> acontecimientosValidos = new List<Acontecimiento>();
        string nombreArchivo = Path.GetFileName(rutaArchivo);

        try
        {
            string contenidoJson = File.ReadAllText(rutaArchivo, Encoding.UTF8);
            JsonSerializerOptions opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<AcontecimientoDto> eventos = JsonSerializer.Deserialize<List<AcontecimientoDto>>(contenidoJson, opciones) ?? new List<AcontecimientoDto>();

            foreach (AcontecimientoDto evtDto in eventos)
            {
                try
                {
                    acontecimientosValidos.Add(evtDto.MapToDomain());
                }
                catch (NexoLogException ex)
                {
                    string refEvt = string.IsNullOrWhiteSpace(evtDto.IdAcontecimiento) ? "EVENTO_INCOMPLETO" : evtDto.IdAcontecimiento;
                    inconsistencias.Add(new RegistroInconsistencia(nombreArchivo, refEvt, RegistroInconsistencia.TipoDatoInvalido, ex.Message, DateTime.Now));
                }
            }
        }
        catch (Exception ex)
        {
            inconsistencias.Add(new RegistroInconsistencia(nombreArchivo, "GENERAL", RegistroInconsistencia.TipoDatoInvalido, ex.Message, DateTime.Now));
        }

        return acontecimientosValidos;
    }
}
