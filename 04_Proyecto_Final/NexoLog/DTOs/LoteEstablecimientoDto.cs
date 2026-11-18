using System.Text.Json.Serialization;

namespace NexoLog.DTOs;

public class LoteEstablecimientoDto
{
    [JsonPropertyName("codigoEstablecimiento")]
    public string CodigoEstablecimiento { get; set; } = string.Empty;

    [JsonPropertyName("nombreComercio")]
    public string NombreComercio { get; set; } = string.Empty;

    [JsonPropertyName("solicitudes")]
    public List<PedidoDto> Solicitudes { get; set; } = new List<PedidoDto>();
}
