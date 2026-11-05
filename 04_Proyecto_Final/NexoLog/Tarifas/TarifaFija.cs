namespace NexoLog.Tarifas;

// Tarifa comercial fija por despacho entregado.
public class TarifaFija : ITarifa
{
    // Campos privados
    private decimal _valorFijo;

    // Constructor horizontal
    public TarifaFija(decimal valorFijo)
    {
        ValorFijo = valorFijo;
    }

    // Propiedades publicas
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

        return _valorFijo;
    }

    public string ObtenerTipoTarifa()
    {
        return "Tarifa Fija";
    }
}
