using System.Text.Json.Serialization;

namespace NexoLog.Infraestructura.Lectores.DTOs;

// DTO para deserializar un lote de solicitudes agrupadas por comercio en JSON.
public class LoteEstablecimientoDto
{
    [JsonPropertyName("codigoEstablecimiento")]
    public string CodigoEstablecimiento { get; set; } = string.Empty;

    [JsonPropertyName("nombreComercio")]
    public string NombreComercio { get; set; } = string.Empty;

    [JsonPropertyName("solicitudes")]
    public List<PedidoJsonDto> Solicitudes { get; set; } = new List<PedidoJsonDto>();
}
