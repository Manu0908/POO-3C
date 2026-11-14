using System.Text.Json.Serialization;
using NexoLog.Informes;

namespace NexoLog.DTOs;

// DTO explicito para serializar el paquete consolidado de informes en formato JSON.
public class InformeConsolidadoDto
{
    [JsonPropertyName("fechaGeneracion")]
    public string FechaGeneracion { get; set; } = string.Empty;

    [JsonPropertyName("sistema")]
    public string Sistema { get; set; } = string.Empty;

    [JsonPropertyName("operativo")]
    public InformeOperativo Operativo { get; set; }

    [JsonPropertyName("financiero")]
    public InformeFinanciero Financiero { get; set; }

    [JsonPropertyName("inconsistencias")]
    public InformeInconsistencias Inconsistencias { get; set; }
}
