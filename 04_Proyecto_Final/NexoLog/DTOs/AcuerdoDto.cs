using System.Text.Json.Serialization;

namespace NexoLog.DTOs;

public class AcuerdoDto
{
    [JsonPropertyName("comercio")]
    public string CodigoComercio { get; set; }

    [JsonPropertyName("tarifa")]
    public string TipoTarifa { get; set; }

    [JsonPropertyName("valor")]
    public double TarifaBase { get; set; }
}
