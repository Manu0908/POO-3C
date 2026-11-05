using NexoLog.Tarifas;

namespace NexoLog;

// Representa un comercio o punto remitente de despachos.
public class Establecimiento
{
    // Campos privados
    private ITarifa _tarifaAcordada;
    private List<Despacho> _despachos;

    // Propiedades calculadas / derivadas
    public int CantidadDespachos { get { return _despachos.Count; } }

    // Constructores horizontales
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

    // Propiedades publicas
    public string Codigo { get; private set; }
    public string NombreComercial { get; private set; }
    public ITarifa TarifaAcordada { get { return _tarifaAcordada; } set { _tarifaAcordada = value; } }
    public List<Despacho> Despachos { get { return _despachos; } }

    // Metodos de negocio
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
        string tarifaDesc = _tarifaAcordada != null ? _tarifaAcordada.ObtenerTipoTarifa() : "Sin Tarifa";
        return $"Establecimiento [{Codigo}] - {NombreComercial} | Tarifa: {tarifaDesc} | Despachos: {CantidadDespachos}";
    }
}
