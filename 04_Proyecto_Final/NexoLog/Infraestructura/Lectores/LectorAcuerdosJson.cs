using System.Text;
using System.Text.Json;
using NexoLog.Dominio.Inconsistencias;
using NexoLog.Infraestructura.Lectores.DTOs;

namespace NexoLog.Infraestructura.Lectores;

// Lector resiliente para el archivo de acuerdos comerciales en formato JSON.
public class LectorAcuerdosJson : ILectorArchivo<AcuerdoDto>
{
    public List<AcuerdoDto> Leer(string rutaArchivo, List<RegistroInconsistencia> inconsistencias)
    {
        List<AcuerdoDto> acuerdosValidos = new List<AcuerdoDto>();
        string nombreArchivo = Path.GetFileName(rutaArchivo);

        if (!File.Exists(rutaArchivo))
        {
            inconsistencias.Add(new RegistroInconsistencia(
                nombreArchivo,
                "GENERAL",
                RegistroInconsistencia.TipoReferenciaInexistente,
                $"El archivo de acuerdos '{rutaArchivo}' no existe.",
                DateTime.Now));
            return acuerdosValidos;
        }

        try
        {
            string contenidoJson = File.ReadAllText(rutaArchivo, Encoding.UTF8);
            JsonSerializerOptions opciones = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            List<AcuerdoDto> acuerdosLeidos = JsonSerializer.Deserialize<List<AcuerdoDto>>(contenidoJson, opciones);

            if (acuerdosLeidos == null || acuerdosLeidos.Count == 0)
            {
                inconsistencias.Add(new RegistroInconsistencia(
                    nombreArchivo,
                    "GENERAL",
                    RegistroInconsistencia.TipoDatoInvalido,
                    "El archivo de acuerdos está vacío o no contiene registros válidos.",
                    DateTime.Now));
                return acuerdosValidos;
            }

            foreach (AcuerdoDto acuerdo in acuerdosLeidos)
            {
                if (string.IsNullOrWhiteSpace(acuerdo.CodigoComercio))
                {
                    inconsistencias.Add(new RegistroInconsistencia(
                        nombreArchivo,
                        "ACUERDO_SIN_CODIGO",
                        RegistroInconsistencia.TipoDatoInvalido,
                        "El acuerdo comercial carece de código de comercio.",
                        DateTime.Now));
                    continue;
                }

                if (acuerdo.TarifaBase <= 0)
                {
                    inconsistencias.Add(new RegistroInconsistencia(
                        nombreArchivo,
                        acuerdo.CodigoComercio,
                        RegistroInconsistencia.TipoDatoInvalido,
                        $"La tarifa base para {acuerdo.CodigoComercio} debe ser mayor a cero.",
                        DateTime.Now));
                    continue;
                }

                string tipoTarifaMayus = (acuerdo.TipoTarifa ?? string.Empty).Trim().ToUpperInvariant();
                if (tipoTarifaMayus != "PESO" && tipoTarifaMayus != "DISTANCIA" && tipoTarifaMayus != "FIJA")
                {
                    inconsistencias.Add(new RegistroInconsistencia(
                        nombreArchivo,
                        acuerdo.CodigoComercio,
                        RegistroInconsistencia.TipoDatoInvalido,
                        $"Tipo de tarifa desconocido: '{acuerdo.TipoTarifa}'.",
                        DateTime.Now));
                    continue;
                }

                acuerdo.TipoTarifa = tipoTarifaMayus;
                acuerdosValidos.Add(acuerdo);
            }
        }
        catch (Exception ex)
        {
            inconsistencias.Add(new RegistroInconsistencia(
                nombreArchivo,
                "GENERAL",
                RegistroInconsistencia.TipoDatoInvalido,
                $"Error crítico al leer acuerdos: {ex.Message}",
                DateTime.Now));
        }

        return acuerdosValidos;
    }
}
