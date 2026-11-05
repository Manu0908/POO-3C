namespace NexoLog.Dominio.Tarifas;

// Tarifa comercial basada en la distancia recorrida en kilometros.
public class TarifaPorDistancia : ITarifa
{
    private decimal _valorPorKilometro;

    public decimal ValorPorKilometro
    {
        get { return _valorPorKilometro; }
        private set
        {
            if (value <= 0)
            {
                throw new NexoLogException("El valor por kilometro debe ser estrictamente mayor a cero.");
            }
            _valorPorKilometro = value;
        }
    }

    public TarifaPorDistancia(decimal valorPorKilometro)
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
