using System.Text.Json.Serialization;

namespace NexoLog.Infraestructura.Lectores.DTOs;

/// <summary>
/// Objeto de transferencia de datos para deserializar acontecimientos de seguimiento desde JSON.
/// </summary>
public class AcontecimientoJsonDto
{
    [JsonPropertyName("idAcontecimiento")]
    public string IdAcontecimiento { get; set; } = string.Empty;

    [JsonPropertyName("numeroGuia")]
    public string NumeroGuia { get; set; } = string.Empty;

    [JsonPropertyName("tipo")]
    public string Tipo { get; set; } = string.Empty;

    [JsonPropertyName("fechaHora")]
    public DateTime FechaHora { get; set; }

    [JsonPropertyName("observacion")]
    public string Observacion { get; set; } = string.Empty;
}
