using System.Text;
using System.Text.Json;
using NexoLog.DTOs;

namespace NexoLog.Importadores;

// Importador resiliente para acontecimientos y eventos de seguimiento en formato JSON.
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
                string idEvt = evtDto.IdAcontecimiento;
                string guia = evtDto.NumeroGuia;

                if (string.IsNullOrWhiteSpace(idEvt) || string.IsNullOrWhiteSpace(guia))
                {
                    inconsistencias.Add(new RegistroInconsistencia(
                        nombreArchivo,
                        "EVENTO_INCOMPLETO",
                        RegistroInconsistencia.TipoDatoInvalido,
                        "Se detecto un acontecimiento sin identificador o sin numero de guia.",
                        DateTime.Now));
                    continue;
                }

                string tipoLimpio = (evtDto.Tipo ?? string.Empty).Trim().ToUpperInvariant();
                if (tipoLimpio != Acontecimiento.TipoRec &&
                    tipoLimpio != Acontecimiento.TipoRut &&
                    tipoLimpio != Acontecimiento.TipoEnt &&
                    tipoLimpio != Acontecimiento.TipoDev)
                {
                    inconsistencias.Add(new RegistroInconsistencia(
                        nombreArchivo,
                        idEvt,
                        RegistroInconsistencia.TipoDatoInvalido,
                        $"Tipo de acontecimiento desconocido '{evtDto.Tipo}' en el evento '{idEvt}'.",
                        DateTime.Now));
                    continue;
                }

                try
                {
                    Acontecimiento nuevoAcontecimiento = new Acontecimiento(
                        idEvt,
                        guia,
                        tipoLimpio,
                        evtDto.FechaHora);

                    acontecimientosValidos.Add(nuevoAcontecimiento);
                }
                catch (NexoLogException ex)
                {
                    inconsistencias.Add(new RegistroInconsistencia(
                        nombreArchivo,
                        idEvt,
                        RegistroInconsistencia.TipoDatoInvalido,
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
                RegistroInconsistencia.TipoDatoInvalido,
                $"Error critico al procesar el archivo JSON de seguimiento: {ex.Message}",
                DateTime.Now));
        }

        return acontecimientosValidos;
    }
}
