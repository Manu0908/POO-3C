using System.Text.Json.Serialization;

namespace NexoLog.DTOs;

public class RaizPedidosDto
{
    [JsonPropertyName("comercios")]
    public List<LoteEstablecimientoDto> Comercios { get; set; } = new List<LoteEstablecimientoDto>();
}

public class LoteEstablecimientoDto
{
    [JsonPropertyName("comercio")]
    public string CodigoEstablecimiento { get; set; }

    [JsonPropertyName("nombre")]
    public string NombreComercio { get; set; }

    [JsonPropertyName("solicitudes")]
    public List<PedidoDto> Solicitudes { get; set; } = new List<PedidoDto>();
}
