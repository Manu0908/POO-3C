using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using NexoLog.DTOs;
using NexoLog.Informes;

namespace NexoLog.Exportadores;

public class ExportadorInformesJson : IExportadorInforme
{
    public string Extension { get { return "json"; } }

    public void Exportar(InformeOperativo operativo, InformeFinanciero financiero, InformeInconsistencias inconsistencias, string rutaDestino)
    {
        string directorio = Path.GetDirectoryName(rutaDestino);
        if (!string.IsNullOrWhiteSpace(directorio) && !Directory.Exists(directorio))
        {
            Directory.CreateDirectory(directorio);
        }

        InformeConsolidadoDto consolidado = InformeConsolidadoDto.MapToDto(operativo, financiero, inconsistencias);

        JsonSerializerOptions opciones = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        string jsonSalida = JsonSerializer.Serialize(consolidado, opciones);
        File.WriteAllText(rutaDestino, jsonSalida, Encoding.UTF8);
    }
}
