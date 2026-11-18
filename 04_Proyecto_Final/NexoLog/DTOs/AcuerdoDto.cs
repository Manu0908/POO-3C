using System.Text.Json.Serialization;
using NexoLog.Tarifas;

namespace NexoLog.DTOs;

public class AcuerdoDto
{
    [JsonPropertyName("comercio")]
    public string CodigoComercio { get; set; } = string.Empty;

    [JsonPropertyName("tarifa")]
    public string TipoTarifa { get; set; } = string.Empty;

    [JsonPropertyName("valor")]
    public decimal TarifaBase { get; set; }

    public ITarifa CrearTarifa()
    {
        string tipo = (TipoTarifa ?? string.Empty).Trim().ToUpper();
        return tipo switch
        {
            "PESO" => new TarifaPorPeso(TarifaBase),
            "DISTANCIA" => new TarifaPorDistancia(TarifaBase),
            "FIJA" => new TarifaFija(TarifaBase),
            _ => throw new NexoLogException($"Modalidad de tarifa desconocida '{TipoTarifa}' para el comercio {CodigoComercio}.")
        };
    }

    public Establecimiento MapToDomain()
    {
        ITarifa tarifa = CrearTarifa();
        string nombreInicial = string.IsNullOrWhiteSpace(CodigoComercio) ? string.Empty : $"Comercio {CodigoComercio.Trim()}";
        return new Establecimiento(CodigoComercio, nombreInicial, tarifa);
    }
}
