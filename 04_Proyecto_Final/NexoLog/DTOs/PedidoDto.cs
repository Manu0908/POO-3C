using System.Text.Json.Serialization;

namespace NexoLog.DTOs;

public class ProgramacionDto
{
    [JsonPropertyName("inicio")]
    public DateTime Inicio { get; set; }

    [JsonPropertyName("fin")]
    public DateTime Fin { get; set; }
}

public class PedidoDto
{
    [JsonPropertyName("guia")]
    public string NumeroGuia { get; set; }

    [JsonPropertyName("servicio")]
    public string Modalidad { get; set; }

    [JsonPropertyName("transportadora")]
    public string Transportadora { get; set; }

    [JsonPropertyName("destinatario")]
    public string Destinatario { get; set; }

    [JsonPropertyName("ciudad")]
    public string CiudadDestino { get; set; }

    [JsonPropertyName("distanciaKm")]
    public double DistanciaKm { get; set; }

    [JsonPropertyName("programacion")]
    public ProgramacionDto Programacion { get; set; }

    [JsonPropertyName("paquetes")]
    public List<PaqueteDto> Paquetes { get; set; } = new List<PaqueteDto>();
}
