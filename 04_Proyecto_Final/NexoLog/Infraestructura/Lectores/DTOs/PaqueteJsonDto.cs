using System.Text.Json.Serialization;

namespace NexoLog.Infraestructura.Lectores.DTOs;

// DTO para deserializar información de un paquete individual.
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
