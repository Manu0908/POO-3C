using System.Text.Json.Serialization;

namespace NexoLog.DTOs;

public class AcontecimientoDto
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

    public Acontecimiento MapToDomain()
    {
        return new Acontecimiento(IdAcontecimiento, NumeroGuia, Tipo, FechaHora);
    }
}
