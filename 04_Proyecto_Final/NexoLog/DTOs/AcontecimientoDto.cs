using System.Text.Json.Serialization;

namespace NexoLog.DTOs;

public class AcontecimientoDto
{
    [JsonPropertyName("id")]
    public string IdAcontecimiento { get; set; }

    [JsonPropertyName("guia")]
    public string NumeroGuia { get; set; }

    [JsonPropertyName("codigo")]
    public string Tipo { get; set; }

    [JsonPropertyName("fecha")]
    public DateTime FechaHora { get; set; }
}
