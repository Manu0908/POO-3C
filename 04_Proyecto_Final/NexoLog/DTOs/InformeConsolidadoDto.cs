using System.Text.Json.Serialization;
using NexoLog.Informes;

namespace NexoLog.DTOs;

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

    public static InformeConsolidadoDto MapToDto(InformeOperativo operativo, InformeFinanciero financiero, InformeInconsistencias inconsistencias)
    {
        return new InformeConsolidadoDto
        {
            FechaGeneracion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            Sistema = "NexoLog S.A.S.",
            Operativo = operativo,
            Financiero = financiero,
            Inconsistencias = inconsistencias
        };
    }
}
