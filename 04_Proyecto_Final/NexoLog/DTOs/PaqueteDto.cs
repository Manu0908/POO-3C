using System.Text.Json.Serialization;

namespace NexoLog.DTOs;

public class PaqueteDto
{
    [JsonPropertyName("codigo")]
    public string IdPaquete { get; set; } = string.Empty;

    [JsonPropertyName("descripcion")]
    public string Descripcion { get; set; } = string.Empty;

    [JsonPropertyName("pesoKg")]
    public double Peso { get; set; }

    [JsonPropertyName("valor")]
    public decimal ValorDeclarado { get; set; }

    public Paquete MapToDomain()
    {
        return new Paquete(IdPaquete, Descripcion, Peso, ValorDeclarado);
    }
}
