namespace NexoLog.Tarifas;

public class TarifaFija : ITarifa
{
    private double _valorFijo;

    public string TipoTarifa => "Tarifa Fija";
    public double ValorBase => _valorFijo;

    private TarifaFija() { }

    public TarifaFija(double valorFijo)
    {
        ValorFijo = valorFijo;
    }

    public double ValorFijo
    {
        get { return _valorFijo; }
        private set
        {
            if (value <= 0)
            {
                throw new NexoLogException("El valor de la tarifa fija debe ser estrictamente mayor a cero.");
            }
            _valorFijo = value;
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

        return _valorFijo;
    }
}
