using NexoLog.Servicios;

namespace NexoLog;

public class Despacho
{
    public const string EstadoPendiente = "Pendiente";
    public const string EstadoEnDistribucion = "EnDistribucion";
    public const string EstadoEntregado = "Entregado";
    public const string EstadoDevuelto = "Devuelto";

    private string _numeroGuia;
    private double _distanciaKm;
    private string _estadoActual;
    private ModalidadServicio _modalidad;
    private List<Paquete> _paquetes;
    private List<Acontecimiento> _acontecimientos;

    public int CantidadPaquetes { get { return _paquetes.Count; } }

    private Despacho() { }

    public Despacho(string numeroGuia, string codigoEstablecimiento, string destinatario, string ciudadDestino, double distanciaKm, string transportadora)
    {
        NumeroGuia = numeroGuia;
        CodigoEstablecimiento = codigoEstablecimiento;
        Destinatario = destinatario;
        CiudadDestino = ciudadDestino;
        DistanciaKm = distanciaKm;
        Transportadora = transportadora;
        _estadoActual = EstadoPendiente;
        _paquetes = new List<Paquete>();
        _acontecimientos = new List<Acontecimiento>();
    }

    public string CodigoEstablecimiento { get; private set; }
    public string Destinatario { get; private set; }
    public string CiudadDestino { get; private set; }
    public string Transportadora { get; private set; }
    public string EstadoActual { get { return _estadoActual; } private set { _estadoActual = value; } }
    public ModalidadServicio Modalidad { get { return _modalidad; } private set { _modalidad = value; } }
    public List<Paquete> Paquetes { get { return new List<Paquete>(_paquetes); } }
    public List<Acontecimiento> Acontecimientos { get { return new List<Acontecimiento>(_acontecimientos); } }

    public string NumeroGuia
    {
        get { return _numeroGuia; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NexoLogException("El numero de guia es obligatorio.");
            }
            _numeroGuia = value.Trim();
        }
    }

    public double DistanciaKm
    {
        get { return _distanciaKm; }
        private set
        {
            if (value <= 0)
            {
                throw new NexoLogException("La distancia en kilometros debe ser mayor a 0.");
            }
            _distanciaKm = value;
        }
    }

    public void AsignarModalidad(ModalidadServicio modalidad)
    {
        _modalidad = modalidad;
    }

    public void AgregarPaquete(Paquete paquete)
    {
        if (paquete == null)
        {
            throw new NexoLogException("No se puede agregar un paquete nulo al despacho.");
        }

        if (_paquetes.Any(p => p.IdPaquete.ToLower() == paquete.IdPaquete.ToLower()))
        {
            throw new NexoLogException($"El paquete con ID '{paquete.IdPaquete}' ya se encuentra registrado en el despacho {NumeroGuia}.");
        }

        _paquetes.Add(paquete);
    }

    public double CalcularPesoTotal()
    {
        return _paquetes.Sum(x => x.Peso);
    }

    public bool EsFacturable()
    {
        return _estadoActual == EstadoEntregado;
    }

    public bool EvaluarCumplimiento()
    {
        if (_modalidad != null && _estadoActual == EstadoEntregado)
        {
            return _modalidad.EvaluarCumplimiento(this);
        }
        return false;
    }

    public void AgregarAcontecimiento(Acontecimiento acontecimiento)
    {
        if (acontecimiento == null)
        {
            throw new NexoLogException("El acontecimiento no puede ser nulo.");
        }

        if (!PrimerAcontecimientoValido(acontecimiento) || !AcontecimientoValido(acontecimiento))
        {
            throw new NexoLogException($"El acontecimiento '{acontecimiento.Tipo}' no es valido para el estado '{_estadoActual}'.");
        }

        _acontecimientos.Add(acontecimiento);
        _estadoActual = acontecimiento.Tipo switch
        {
            Acontecimiento.TipoRut => EstadoEnDistribucion,
            Acontecimiento.TipoEnt => EstadoEntregado,
            Acontecimiento.TipoDev => EstadoDevuelto,
            _ => _estadoActual
        };
    }

    private bool PrimerAcontecimientoValido(Acontecimiento acontecimiento)
    {
        if (_acontecimientos.Count == 0)
        {
            return acontecimiento.Tipo == Acontecimiento.TipoRec;
        }
        return true;
    }

    private bool AcontecimientoValido(Acontecimiento acontecimiento)
    {
        if (_acontecimientos.Count == 0)
        {
            return true;
        }

        Acontecimiento ultimoAcontecimiento = _acontecimientos.Last();
        return ultimoAcontecimiento.Tipo switch
        {
            Acontecimiento.TipoRec => acontecimiento.Tipo == Acontecimiento.TipoRut,
            Acontecimiento.TipoRut => acontecimiento.Tipo == Acontecimiento.TipoEnt || acontecimiento.Tipo == Acontecimiento.TipoDev,
            Acontecimiento.TipoEnt => false,
            Acontecimiento.TipoDev => false,
            _ => false
        };
    }

    public Acontecimiento BuscarAcontecimiento(string tipo)
    {
        return _acontecimientos.FirstOrDefault(a => a.Tipo == tipo);
    }

    public string MostrarInformacion()
    {
        string modalidadNombre = _modalidad != null ? _modalidad.NombreModalidad : "Sin Asignar";
        return $"Guia: {NumeroGuia} | Modalidad: {modalidadNombre} | Destino: {CiudadDestino} ({Destinatario}) | Distancia: {DistanciaKm:F1} km | Estado: {EstadoActual} | Paquetes: {CantidadPaquetes} ({CalcularPesoTotal():F2} kg)";
    }

    public bool TieneMismosDatosQue(Despacho otroDespacho)
    {
        if (otroDespacho == null)
        {
            return false;
        }

        return CodigoEstablecimiento.ToLower() == otroDespacho.CodigoEstablecimiento.ToLower()
            && Destinatario.ToLower() == otroDespacho.Destinatario.ToLower()
            && CiudadDestino.ToLower() == otroDespacho.CiudadDestino.ToLower()
            && Math.Abs(DistanciaKm - otroDespacho.DistanciaKm) <= 0.001
            && Transportadora.ToLower() == otroDespacho.Transportadora.ToLower()
            && CantidadPaquetes == otroDespacho.CantidadPaquetes
            && Math.Abs(CalcularPesoTotal() - otroDespacho.CalcularPesoTotal()) <= 0.001;
    }
}
