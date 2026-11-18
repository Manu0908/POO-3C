using NexoLog.Tarifas;

namespace NexoLog;

public class Establecimiento
{
    private string _codigo;
    private string _nombreComercial;
    private ITarifa _tarifaAcordada;
    private List<Despacho> _despachos;

    public int CantidadDespachos { get { return _despachos.Count; } }

    private Establecimiento() { }

    public Establecimiento(string codigo, string nombreComercial)
    {
        Codigo = codigo;
        NombreComercial = nombreComercial;
        _despachos = new List<Despacho>();
    }

    public Establecimiento(string codigo, string nombreComercial, ITarifa tarifaAcordada) : this(codigo, nombreComercial)
    {
        _tarifaAcordada = tarifaAcordada;
    }

    public string Codigo
    {
        get { return _codigo; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NexoLogException("El codigo del establecimiento es obligatorio.");
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

    public ITarifa TarifaAcordada
    {
        get { return _tarifaAcordada; }
        private set { _tarifaAcordada = value; }
    }

    public List<Despacho> Despachos
    {
        get { return new List<Despacho>(_despachos); }
    }

    public void AsignarTarifa(ITarifa tarifa)
    {
        _tarifaAcordada = tarifa;
    }

    public void AgregarDespacho(Despacho despacho)
    {
        if (despacho == null)
        {
            throw new NexoLogException("No se puede agregar un despacho nulo al establecimiento.");
        }

        foreach (Despacho d in _despachos)
        {
            if (d.NumeroGuia.ToLower() == despacho.NumeroGuia.ToLower())
            {
                throw new NexoLogException($"Ya existe un despacho con numero de guia '{despacho.NumeroGuia}' en el establecimiento {NombreComercial}.");
            }
        }

        _despachos.Add(despacho);
    }

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

    public string MostrarInformacion()
    {
        string tarifaDesc = _tarifaAcordada != null ? _tarifaAcordada.TipoTarifa : "Sin Tarifa";
        return $"Establecimiento [{Codigo}] - {NombreComercial} | Tarifa: {tarifaDesc} | Despachos: {CantidadDespachos}";
    }
}
