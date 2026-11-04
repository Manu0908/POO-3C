using NexoLog.Dominio.Tarifas;

namespace NexoLog.Dominio;

// Representa una sucursal, tienda o punto de expedición remitente.
public class Establecimiento
{
    private string _codigo;
    private string _nombreComercial;
    private IEstrategiaCobro _tarifaAcordada;
    private List<Despacho> _despachos;

    public string Codigo
    {
        get { return _codigo; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NexoLogException("El código del establecimiento es obligatorio.");
            }
            _codigo = value.Trim();
        }
    }

    public string NombreComercial
    {
        get { return _nombreComercial; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NexoLogException("El nombre comercial del establecimiento es obligatorio.");
            }
            _nombreComercial = value.Trim();
        }
    }

    public IEstrategiaCobro TarifaAcordada
    {
        get { return _tarifaAcordada; }
        set { _tarifaAcordada = value; }
    }

    public IReadOnlyList<Despacho> Despachos
    {
        get { return _despachos.AsReadOnly(); }
    }

    public int CantidadDespachos
    {
        get { return _despachos.Count; }
    }

    public Establecimiento(string codigo, string nombreComercial)
    {
        Codigo = codigo;
        NombreComercial = nombreComercial;
        _tarifaAcordada = null;
        _despachos = new List<Despacho>();
    }

    public Establecimiento(string codigo, string nombreComercial, IEstrategiaCobro tarifaAcordada)
        : this(codigo, nombreComercial)
    {
        _tarifaAcordada = tarifaAcordada;
    }

    // Asigna o modifica la estrategia de cobro comercial pactada con el establecimiento
    public void AsignarTarifa(IEstrategiaCobro tarifa)
    {
        _tarifaAcordada = tarifa;
    }

    // Registra un nuevo despacho en el establecimiento evitando duplicidad de número de guía
    public void AgregarDespacho(Despacho despacho)
    {
        if (despacho == null)
        {
            throw new NexoLogException("No se puede agregar un despacho nulo al establecimiento.");
        }

        foreach (Despacho d in _despachos)
        {
            if (string.Equals(d.NumeroGuia, despacho.NumeroGuia, StringComparison.OrdinalIgnoreCase))
            {
                throw new NexoLogException($"Ya existe un despacho registrado con el número de guía '{despacho.NumeroGuia}' en el establecimiento {NombreComercial}.");
            }
        }

        _despachos.Add(despacho);
    }

    // Calcula el importe total facturable consolidado de todos los despachos entregados
    public decimal CalcularTotalFacturable()
    {
        if (_tarifaAcordada == null)
        {
            return 0m;
        }

        decimal totalFacturable = 0m;
        foreach (Despacho despacho in _despachos)
        {
            if (despacho.EsFacturable())
            {
                totalFacturable += _tarifaAcordada.CalcularTarifa(despacho);
            }
        }

        return totalFacturable;
    }

    public override string ToString()
    {
        string tarifaDesc = _tarifaAcordada != null ? _tarifaAcordada.ObtenerTipoTarifa() : "Sin Tarifa";
        return $"Establecimiento [{Codigo}] - {NombreComercial} | Tarifa: {tarifaDesc} | Despachos: {CantidadDespachos}";
    }
}
