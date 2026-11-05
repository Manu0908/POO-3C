using System.Text.Json.Serialization;

namespace NexoLog.DTOs;

// DTO para deserializar paquetes individuales desde JSON.
public class PaqueteJsonDto
{
    [JsonPropertyName("idPaquete")]
    public string IdPaquete { get; set; } = string.Empty;

    [JsonPropertyName("descripcion")]
    public string Descripcion { get; set; } = string.Empty;

    [JsonPropertyName("peso")]
    public double Peso { get; set; }

    [JsonPropertyName("valorDeclarado")]
    public decimal ValorDeclarado { get; set; }
}
