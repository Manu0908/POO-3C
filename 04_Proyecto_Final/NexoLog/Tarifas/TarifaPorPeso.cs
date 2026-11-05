namespace NexoLog.Tarifas;

// Tarifa comercial calculada segun el peso acumulado en kilogramos.
public class TarifaPorPeso : ITarifa
{
    // Campos privados
    private decimal _valorPorKilogramo;

    // Constructor horizontal
    public TarifaPorPeso(decimal valorPorKilogramo)
    {
        ValorPorKilogramo = valorPorKilogramo;
    }

    // Propiedades publicas
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

        decimal pesoTotal = (decimal)despacho.CalcularPesoTotal();
        return pesoTotal * _valorPorKilogramo;
    }

    public string ObtenerTipoTarifa()
    {
        return "Tarifa por Peso";
    }
}
