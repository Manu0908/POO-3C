using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using NexoLog.DTOs;

namespace NexoLog.Exportadores;

public class ExportadorInformesJson : IExportadorInforme
{
    public string Extension { get { return "json"; } }

    public void Exportar(InformeConsolidadoDto dto, string rutaDestino)
    {
        string directorio = Path.GetDirectoryName(rutaDestino);
        if (!string.IsNullOrWhiteSpace(directorio) && !Directory.Exists(directorio))
        {
            Directory.CreateDirectory(directorio);
        }

        JsonSerializerOptions opciones = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        string jsonSalida = JsonSerializer.Serialize(dto, opciones);
        File.WriteAllText(rutaDestino, jsonSalida, Encoding.UTF8);
    }
}
