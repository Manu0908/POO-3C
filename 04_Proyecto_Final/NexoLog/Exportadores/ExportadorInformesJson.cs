using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using NexoLog.Informes;

namespace NexoLog.Exportadores;

// Exportador de informes consolidados a formato JSON con codificacion UTF-8.
public class ExportadorInformesJson : IExportadorInforme
{
    public void Exportar(InformeOperativo operativo, InformeFinanciero financiero, InformeInconsistencias inconsistencias, string rutaDestino)
    {
        string directorio = Path.GetDirectoryName(rutaDestino);
        if (!string.IsNullOrWhiteSpace(directorio) && !Directory.Exists(directorio))
        {
            Directory.CreateDirectory(directorio);
        }

        var consolidado = new
        {
            fechaGeneracion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            sistema = "NexoLog S.A.S.",
            operativo = operativo,
            financiero = financiero,
            inconsistencias = inconsistencias
        };

        JsonSerializerOptions opciones = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        string jsonSalida = JsonSerializer.Serialize(consolidado, opciones);
        File.WriteAllText(rutaDestino, jsonSalida, Encoding.UTF8);
    }
}
