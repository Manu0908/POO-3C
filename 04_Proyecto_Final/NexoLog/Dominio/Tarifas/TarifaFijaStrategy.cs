namespace NexoLog.Dominio.Tarifas;

// Estrategia de cobro basada en una tarifa fija por despacho entregado.
public class TarifaFijaStrategy : IEstrategiaCobro
{
    private decimal _tarifaFija;

    public decimal TarifaFija
    {
        get { return _tarifaFija; }
        private set
        {
            if (value <= 0)
            {
                throw new NexoLogException("El valor de la tarifa fija debe ser estrictamente mayor a cero.");
            }
            _tarifaFija = value;
        }
    }

    public TarifaFijaStrategy(decimal tarifaFija)
    {
        TarifaFija = tarifaFija;
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

        return _tarifaFija;
    }

    public string ObtenerTipoTarifa()
    {
        return "Tarifa Fija";
    }
}
