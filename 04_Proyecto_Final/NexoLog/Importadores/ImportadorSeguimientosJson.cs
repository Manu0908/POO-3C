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

        if (!File.Exists(rutaArchivo))
        {
            inconsistencias.Add(new RegistroInconsistencia(
                nombreArchivo,
                "GENERAL",
                RegistroInconsistencia.TipoReferenciaInexistente,
                $"El archivo de seguimiento '{rutaArchivo}' no existe.",
                DateTime.Now));
            return acontecimientosValidos;
        }

        try
        {
            string contenidoJson = File.ReadAllText(rutaArchivo, Encoding.UTF8);
            JsonSerializerOptions opciones = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            List<AcontecimientoJsonDto> listaEventos = JsonSerializer.Deserialize<List<AcontecimientoJsonDto>>(contenidoJson, opciones);

            if (listaEventos == null || listaEventos.Count == 0)
            {
                inconsistencias.Add(new RegistroInconsistencia(
                    nombreArchivo,
                    "GENERAL",
                    RegistroInconsistencia.TipoDatoInvalido,
                    "El archivo de seguimiento JSON no contiene acontecimientos.",
                    DateTime.Now));
                return acontecimientosValidos;
            }

            foreach (AcontecimientoJsonDto evtDto in listaEventos)
            {
                try
                {
                    Acontecimiento nuevoAcontecimiento = evtDto.MapToDomain();
                    acontecimientosValidos.Add(nuevoAcontecimiento);
                }
                catch (NexoLogException ex)
                {
                    string referencia = string.IsNullOrWhiteSpace(evtDto.IdAcontecimiento) ? "EVENTO_INCOMPLETO" : evtDto.IdAcontecimiento;
                    inconsistencias.Add(new RegistroInconsistencia(
                        nombreArchivo,
                        referencia,
                        RegistroInconsistencia.TipoDatoInvalido,
                        ex.Message,
                        DateTime.Now));
                }
            }
        }
        catch (Exception ex)
        {
            inconsistencias.Add(new RegistroInconsistencia(
                nombreArchivo,
                "GENERAL",
                RegistroInconsistencia.TipoDatoInvalido,
                $"Error critico al procesar el archivo JSON de seguimiento: {ex.Message}",
                DateTime.Now));
        }

        return acontecimientosValidos;
    }
}
