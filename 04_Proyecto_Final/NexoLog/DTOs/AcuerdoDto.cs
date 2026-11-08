using System.Text.Json.Serialization;
using NexoLog.Tarifas;

namespace NexoLog.DTOs;

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

    public ITarifa CrearTarifa()
    {
        string tipo = (TipoTarifa ?? string.Empty).Trim().ToUpperInvariant();
        if (tipo == "PESO")
        {
            return new TarifaPorPeso(TarifaBase);
        }
        if (tipo == "DISTANCIA")
        {
            return new TarifaPorDistancia(TarifaBase);
        }
        if (tipo == "FIJA")
        {
            return new TarifaFija(TarifaBase);
        }

        throw new NexoLogException($"Modalidad de tarifa desconocida '{TipoTarifa}' para el comercio {CodigoComercio}.");
    }

    public Establecimiento MapToDomain()
    {
        ITarifa tarifa = CrearTarifa();
        return new Establecimiento(CodigoComercio.Trim(), NombreComercio.Trim(), tarifa);
    }
}
