namespace NexoLog.Tarifas;

public class TarifaPorPeso : ITarifa
{
    private double _valorPorKilogramo;

    public string TipoTarifa => "Tarifa por Peso";
    public double ValorBase => _valorPorKilogramo;

    private TarifaPorPeso() { }

    public TarifaPorPeso(double valorPorKilogramo)
    {
        ValorPorKilogramo = valorPorKilogramo;
    }

    public double ValorPorKilogramo
    {
        get { return _valorPorKilogramo; }
        private set
        {
            if (value <= 0)
            {
                throw new NexoLogException("El valor por kilogramo debe ser estrictamente mayor a cero.");
            }
            _valorPorKilogramo = value;
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

        double pesoTotal = despacho.CalcularPesoTotal();
        return pesoTotal * _valorPorKilogramo;
    }
}
