using System.Text.Json.Serialization;

namespace NexoLog.DTOs;

public class AcontecimientoDto
{
    [JsonPropertyName("id")]
    public string IdAcontecimiento { get; set; } = string.Empty;

    [JsonPropertyName("guia")]
    public string NumeroGuia { get; set; } = string.Empty;

    [JsonPropertyName("codigo")]
    public string Tipo { get; set; } = string.Empty;

    [JsonPropertyName("fecha")]
    public DateTime FechaHora { get; set; }

    public Acontecimiento MapToDomain()
    {
        return new Acontecimiento(IdAcontecimiento, NumeroGuia, Tipo, FechaHora);
    }
}
