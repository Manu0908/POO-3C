namespace NexoLog.Dominio.Tarifas;

// Tarifa comercial basada en el peso acumulado en kilogramos.
public class TarifaPorPeso : ITarifa
{
    private decimal _valorPorKilogramo;

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

    public TarifaPorPeso(decimal valorPorKilogramo)
    {
        ValorPorKilogramo = valorPorKilogramo;
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

    public string ObtenerTipoTarifa()
    {
        return "Tarifa por Peso";
    }
}
