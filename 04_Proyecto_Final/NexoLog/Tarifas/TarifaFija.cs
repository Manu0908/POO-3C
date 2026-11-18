namespace NexoLog.Tarifas;

public class TarifaFija : ITarifa
{
    private decimal _valorFijo;

    public string TipoTarifa { get { return "Tarifa Fija"; } }
    public decimal ValorBase { get { return _valorFijo; } }

    private TarifaFija() { }

    public TarifaFija(decimal valorFijo)
    {
        ValorFijo = valorFijo;
    }

    public decimal ValorFijo
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

        return _valorFijo;
    }
}
