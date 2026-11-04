using System.Text.Json.Serialization;

namespace NexoLog.Infraestructura.Lectores.DTOs;

// DTO para deserializar solicitudes de despacho desde JSON.
public class PedidoJsonDto
{
    [JsonPropertyName("numeroGuia")]
    public string NumeroGuia { get; set; } = string.Empty;

    [JsonPropertyName("destinatario")]
    public string Destinatario { get; set; } = string.Empty;

    [JsonPropertyName("ciudadDestino")]
    public string CiudadDestino { get; set; } = string.Empty;

    [JsonPropertyName("distanciaKm")]
    public double DistanciaKm { get; set; }

    [JsonPropertyName("transportadora")]
    public string Transportadora { get; set; } = string.Empty;

    [JsonPropertyName("modalidad")]
    public string Modalidad { get; set; } = string.Empty;

    [JsonPropertyName("fechaProgramada")]
    public string FechaProgramada { get; set; } = string.Empty;

    [JsonPropertyName("horaInicio")]
    public string HoraInicio { get; set; } = string.Empty;

    [JsonPropertyName("horaFin")]
    public string HoraFin { get; set; } = string.Empty;

    [JsonPropertyName("paquetes")]
    public List<PaqueteJsonDto> Paquetes { get; set; } = new List<PaqueteJsonDto>();
}
