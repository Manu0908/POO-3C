using System.Text.Json.Serialization;

namespace NexoLog.DTOs;

// DTO para deserializar un lote de solicitudes de un comercio desde JSON.
public class LoteEstablecimientoDto
{
    [JsonPropertyName("codigoEstablecimiento")]
    public string CodigoEstablecimiento { get; set; } = string.Empty;

    [JsonPropertyName("nombreComercio")]
    public string NombreComercio { get; set; } = string.Empty;

    [JsonPropertyName("solicitudes")]
    public List<PedidoJsonDto> Solicitudes { get; set; } = new List<PedidoJsonDto>();
}
