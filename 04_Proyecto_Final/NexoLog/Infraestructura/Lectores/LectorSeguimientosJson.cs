using System.Text;
using System.Text.Json;
using NexoLog.Dominio;
using NexoLog.Dominio.Inconsistencias;
using NexoLog.Infraestructura.Lectores.DTOs;

namespace NexoLog.Infraestructura.Lectores;

/// <summary>
/// Lector resiliente para acontecimientos y eventos de seguimiento en formato JSON.
/// </summary>
public class LectorSeguimientosJson : ILectorArchivo<Acontecimiento>
{
    public List<Acontecimiento> Leer(string rutaArchivo, List<RegistroInconsistencia> inconsistencias)
    {
        List<Acontecimiento> acontecimientosValidos = new List<Acontecimiento>();
        string nombreArchivo = Path.GetFileName(rutaArchivo);

        if (!File.Exists(rutaArchivo))
        {
            inconsistencias.Add(new RegistroInconsistencia(
                nombreArchivo,
                "GENERAL",
                TipoInconsistencia.ReferenciaInexistente,
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
                    TipoInconsistencia.DatoInvalido,
                    "El archivo de seguimiento JSON no contiene acontecimientos.",
                    DateTime.Now));
                return acontecimientosValidos;
            }

            foreach (AcontecimientoJsonDto evtDto in listaEventos)
            {
                string idEvt = evtDto.IdAcontecimiento;
                string guia = evtDto.NumeroGuia;

                if (string.IsNullOrWhiteSpace(idEvt) || string.IsNullOrWhiteSpace(guia))
                {
                    inconsistencias.Add(new RegistroInconsistencia(
                        nombreArchivo,
                        "EVENTO_INCOMPLETO",
                        TipoInconsistencia.DatoInvalido,
                        "Se detectó un acontecimiento sin identificador o sin número de guía.",
                        DateTime.Now));
                    continue;
                }

                if (!Enum.TryParse(evtDto.Tipo, true, out TipoAcontecimiento tipoAcontecimiento))
                {
                    inconsistencias.Add(new RegistroInconsistencia(
                        nombreArchivo,
                        idEvt,
                        TipoInconsistencia.DatoInvalido,
                        $"Tipo de acontecimiento desconocido '{evtDto.Tipo}' en el evento '{idEvt}'.",
                        DateTime.Now));
                    continue;
                }

                try
                {
                    Acontecimiento nuevoAcontecimiento = new Acontecimiento(
                        idEvt,
                        guia,
                        tipoAcontecimiento,
                        evtDto.FechaHora);

                    acontecimientosValidos.Add(nuevoAcontecimiento);
                }
                catch (NexoLogException ex)
                {
                    inconsistencias.Add(new RegistroInconsistencia(
                        nombreArchivo,
                        idEvt,
                        TipoInconsistencia.DatoInvalido,
                        $"Error al crear acontecimiento '{idEvt}': {ex.Message}",
                        DateTime.Now));
                }
            }
        }
        catch (Exception ex)
        {
            inconsistencias.Add(new RegistroInconsistencia(
                nombreArchivo,
                "GENERAL",
                TipoInconsistencia.DatoInvalido,
                $"Error crítico al procesar el archivo JSON de seguimiento: {ex.Message}",
                DateTime.Now));
        }

        return acontecimientosValidos;
    }
}
