namespace NexoLog.Dominio.Tarifas;

// Estrategia de cobro basada en la distancia recorrida en kilómetros.
public class TarifaDistanciaStrategy : IEstrategiaCobro
{
    private decimal _valorPorKilometro;

    public decimal ValorPorKilometro
    {
        get { return _valorPorKilometro; }
        private set
        {
            if (value <= 0)
            {
                throw new NexoLogException("El valor por kilómetro debe ser estrictamente mayor a cero.");
            }
            _valorPorKilometro = value;
        }
    }

    public TarifaDistanciaStrategy(decimal valorPorKilometro)
    {
        ValorPorKilometro = valorPorKilometro;
    }

    public decimal CalcularTarifa(Despacho despacho)
    {
        if (despacho == null)
        {
            throw new NexoLogException("No se puede calcular tarifa sobre un despacho nulo.");
        }

        if (!despacho.EsFacturable())
        {
            return 0m;
        }

        decimal distancia = (decimal)despacho.DistanciaKm;
        return distancia * _valorPorKilometro;
    }

    public string ObtenerTipoTarifa()
    {
        return "Tarifa por Distancia";
    }
}
