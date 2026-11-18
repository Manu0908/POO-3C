using NexoLog.DTOs;
using NexoLog.Tarifas;

namespace NexoLog;

public class Establecimiento
{
    private string _codigo;
    private string _nombreComercial;
    private ITarifa _tarifaAcordada;
    private List<Despacho> _despachos;

    public int CantidadDespachos => _despachos.Count;

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

    public void ActualizarNombreComercial(string nombreComercial)
    {
        if (!string.IsNullOrWhiteSpace(nombreComercial))
        {
            NombreComercial = nombreComercial;
        }
    }

    public void AgregarDespacho(Despacho despacho)
    {
        if (despacho == null)
        {
            throw new NexoLogException("No se puede agregar un despacho nulo al establecimiento.");
        }

        if (_despachos.Any(d => d.NumeroGuia.ToLower() == despacho.NumeroGuia.ToLower()))
        {
            throw new NexoLogException($"Ya existe un despacho con numero de guia '{despacho.NumeroGuia}' en el establecimiento {NombreComercial}.");
        }

        _despachos.Add(despacho);
    }

    public double CalcularTotalFacturable()
    {
        if (_tarifaAcordada == null)
        {
            return 0;
        }

        return _despachos.Sum(d => _tarifaAcordada.CalcularTarifa(d));
    }

    public static ITarifa CrearTarifa(string tipoTarifa, double tarifaBase, string codigoComercio)
    {
        if (string.IsNullOrWhiteSpace(tipoTarifa))
        {
            throw new NexoLogException($"Modalidad de tarifa desconocida '{tipoTarifa}' para el comercio {codigoComercio}.");
        }

        string tipo = tipoTarifa.Trim().ToUpper();
        return tipo switch
        {
            "PESO" => new TarifaPorPeso(tarifaBase),
            "DISTANCIA" => new TarifaPorDistancia(tarifaBase),
            "FIJA" => new TarifaFija(tarifaBase),
            _ => throw new NexoLogException($"Modalidad de tarifa desconocida '{tipoTarifa}' para el comercio {codigoComercio}.")
        };
    }

    public static Establecimiento MapToDomain(AcuerdoDto dto)
    {
        if (dto == null)
        {
            throw new NexoLogException("El DTO del acuerdo no puede ser nulo.");
        }

        ITarifa tarifa = CrearTarifa(dto.TipoTarifa, dto.TarifaBase, dto.CodigoComercio);
        string nombreInicial = "";
        if (!string.IsNullOrWhiteSpace(dto.CodigoComercio))
        {
            nombreInicial = $"Comercio {dto.CodigoComercio.Trim()}";
        }
        return new Establecimiento(dto.CodigoComercio, nombreInicial, tarifa);
    }

    public string MostrarInformacion()
    {
        string tarifaDesc = "Sin Tarifa";
        if (_tarifaAcordada != null)
        {
            tarifaDesc = _tarifaAcordada.TipoTarifa;
        }
        return $"Establecimiento [{Codigo}] - {NombreComercial} | Tarifa: {tarifaDesc} | Despachos: {CantidadDespachos}";
    }
}
