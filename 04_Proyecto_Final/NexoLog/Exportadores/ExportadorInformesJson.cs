using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using NexoLog.DTOs;

namespace NexoLog.Exportadores;

public class ExportadorInformesJson : IExportadorInforme
{
    public string TipoArchivo => "json";

    public void Exportar(InformeConsolidadoDto dto)
    {
        var opciones = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        var jsonSalida = JsonSerializer.Serialize(dto, opciones);
        File.WriteAllText("informes_nexolog.json", jsonSalida, Encoding.UTF8);
    }
}
