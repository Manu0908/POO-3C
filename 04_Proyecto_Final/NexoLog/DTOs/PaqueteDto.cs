using System.Text.Json.Serialization;

namespace NexoLog.DTOs;

public class PaqueteDto
{
    [JsonPropertyName("codigo")]
    public string IdPaquete { get; set; }

    [JsonPropertyName("descripcion")]
    public string Descripcion { get; set; }

    [JsonPropertyName("pesoKg")]
    public double Peso { get; set; }

    [JsonPropertyName("valor")]
    public double ValorDeclarado { get; set; }
}
