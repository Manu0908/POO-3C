using System.Text.Json.Serialization;

namespace NexoLog.Infraestructura.Lectores.DTOs;

// DTO para deserializar acuerdos comerciales desde JSON.
public class AcuerdoDto
{
    [JsonPropertyName("codigoComercio")]
    public string CodigoComercio { get; set; } = string.Empty;

    [JsonPropertyName("nombreComercio")]
    public string NombreComercio { get; set; } = string.Empty;

    [JsonPropertyName("tipoTarifa")]
    public string TipoTarifa { get; set; } = string.Empty;

    [JsonPropertyName("tarifaBase")]
    public decimal TarifaBase { get; set; }

    [JsonPropertyName("descripcion")]
    public string Descripcion { get; set; } = string.Empty;
}
