namespace NexoLog.Tarifas;

public class TarifaPorDistancia : ITarifa
{
    private double _valorPorKilometro;

    public string TipoTarifa => "Tarifa por Distancia";
    public double ValorBase => _valorPorKilometro;

    private TarifaPorDistancia() { }

    public TarifaPorDistancia(double valorPorKilometro)
    {
        ValorPorKilometro = valorPorKilometro;
    }

    public double ValorPorKilometro
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

    public double CalcularTarifa(Despacho despacho)
    {
        if (despacho == null)
        {
            throw new NexoLogException("No se puede calcular tarifa sobre un despacho nulo.");
        }

        if (!despacho.EsFacturable())
        {
            return 0;
        }

        double distancia = despacho.DistanciaKm;
        return distancia * _valorPorKilometro;
    }
}
