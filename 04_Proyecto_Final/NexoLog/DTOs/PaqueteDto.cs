using System.Text.Json.Serialization;

namespace NexoLog.DTOs;

public class PaqueteDto
{
    [JsonPropertyName("idPaquete")]
    public string IdPaquete { get; set; } = string.Empty;

    [JsonPropertyName("descripcion")]
    public string Descripcion { get; set; } = string.Empty;

    [JsonPropertyName("peso")]
    public double Peso { get; set; }

    [JsonPropertyName("valorDeclarado")]
    public decimal ValorDeclarado { get; set; }

    public Paquete MapToDomain()
    {
        return new Paquete(IdPaquete, Descripcion, Peso, ValorDeclarado);
    }
}
