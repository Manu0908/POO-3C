namespace NexoLog.Tarifas;

public class TarifaPorPeso : ITarifa
{
    private decimal _valorPorKilogramo;

    public string TipoTarifa { get { return "Tarifa por Peso"; } }
    public decimal ValorBase { get { return _valorPorKilogramo; } }

    private TarifaPorPeso() { }

    public TarifaPorPeso(decimal valorPorKilogramo)
    {
        ValorPorKilogramo = valorPorKilogramo;
    }

    public decimal ValorPorKilogramo
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

        decimal pesoTotal = (decimal)despacho.CalcularPesoTotal();
        return pesoTotal * _valorPorKilogramo;
    }
}
