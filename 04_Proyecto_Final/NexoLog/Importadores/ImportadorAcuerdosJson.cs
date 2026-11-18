using System.Text;
using System.Text.Json;
using NexoLog.DTOs;

namespace NexoLog.Importadores;

public class ImportadorAcuerdosJson : IImportadorArchivo<AcuerdoDto>
{
    public List<AcuerdoDto> Importar(string rutaArchivo, List<RegistroInconsistencia> inconsistencias)
    {
        List<AcuerdoDto> acuerdosValidos = new List<AcuerdoDto>();
        string nombreArchivo = Path.GetFileName(rutaArchivo);

        try
        {
            string contenidoJson = File.ReadAllText(rutaArchivo, Encoding.UTF8);
            JsonSerializerOptions opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<AcuerdoDto> acuerdosLeidos = JsonSerializer.Deserialize<List<AcuerdoDto>>(contenidoJson, opciones) ?? new List<AcuerdoDto>();

            foreach (AcuerdoDto acuerdo in acuerdosLeidos)
            {
                try
                {
                    acuerdo.MapToDomain();
                    acuerdosValidos.Add(acuerdo);
                }
                catch (NexoLogException ex)
                {
                    string refComercio = string.IsNullOrWhiteSpace(acuerdo.CodigoComercio) ? "ACUERDO_SIN_CODIGO" : acuerdo.CodigoComercio;
                    inconsistencias.Add(new RegistroInconsistencia(nombreArchivo, refComercio, RegistroInconsistencia.TipoDatoInvalido, ex.Message, DateTime.Now));
                }
            }
        }
        catch (Exception ex)
        {
            inconsistencias.Add(new RegistroInconsistencia(nombreArchivo, "GENERAL", RegistroInconsistencia.TipoDatoInvalido, ex.Message, DateTime.Now));
        }

        return acuerdosValidos;
    }
}
