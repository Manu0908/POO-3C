using System.Text;
using System.Text.Json;
using NexoLog.DTOs;

namespace NexoLog.Importadores;

public class ImportadorAcuerdosJson : IImportadorArchivo<AcuerdoDto>
{
    public List<AcuerdoDto> Importar(string rutaArchivo, List<RegistroInconsistencia> inconsistencias)
    {
        var acuerdosValidos = new List<AcuerdoDto>();

        try
        {
            string contenidoJson = File.ReadAllText(rutaArchivo, Encoding.UTF8);
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var acuerdosLeidos = JsonSerializer.Deserialize<List<AcuerdoDto>>(contenidoJson, opciones);
            if (acuerdosLeidos == null)
            {
                acuerdosLeidos = new List<AcuerdoDto>();
            }

            foreach (var acuerdo in acuerdosLeidos)
            {
                try
                {
                    Establecimiento.MapToDomain(acuerdo);
                    acuerdosValidos.Add(acuerdo);
                }
                catch (NexoLogException ex)
                {
                    string refComercio = "ACUERDO_SIN_CODIGO";
                    if (!string.IsNullOrWhiteSpace(acuerdo.CodigoComercio))
                    {
                        refComercio = acuerdo.CodigoComercio;
                    }
                    inconsistencias.Add(new RegistroInconsistencia(rutaArchivo, refComercio, RegistroInconsistencia.TipoDatoInvalido, ex.Message, DateTime.Now));
                }
            }
        }
        catch (Exception ex)
        {
            inconsistencias.Add(new RegistroInconsistencia(rutaArchivo, "GENERAL", RegistroInconsistencia.TipoDatoInvalido, ex.Message, DateTime.Now));
        }

        return acuerdosValidos;
    }
}
