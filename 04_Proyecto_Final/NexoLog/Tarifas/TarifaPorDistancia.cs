namespace NexoLog.Tarifas;

// Tarifa comercial calculada segun la distancia recorrida en kilometros.
public class TarifaPorDistancia : ITarifa
{
    // Campos privados
    private decimal _valorPorKilometro;

    // Constructor horizontal
    public TarifaPorDistancia(decimal valorPorKilometro)
    {
        ValorPorKilometro = valorPorKilometro;
    }

    // Propiedades publicas
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

    // Metodos
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
