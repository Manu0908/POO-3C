using NexoLog.Servicios;

namespace NexoLog;

// Representa una orden de despacho o envio logistico.
public class Despacho
{
    // Constantes de estado
    public const string EstadoPendiente = "Pendiente";
    public const string EstadoEnDistribucion = "EnDistribucion";
    public const string EstadoEntregado = "Entregado";
    public const string EstadoDevuelto = "Devuelto";

    // Campos privados con validacion y colecciones
    private string _numeroGuia;
    private double _distanciaKm;
    private string _estadoActual;
    private ModalidadServicio _modalidad;
    private List<Paquete> _paquetes;
    private List<Acontecimiento> _historial;

    // Propiedades calculadas / derivadas
    public int CantidadPaquetes { get { return _paquetes.Count; } }

    // Constructor privado sin parametros para serializacion / reflexion
    private Despacho()
    {
        _numeroGuia = string.Empty;
        CodigoEstablecimiento = string.Empty;
        Destinatario = string.Empty;
        CiudadDestino = string.Empty;
        Transportadora = string.Empty;
        _estadoActual = EstadoPendiente;
        _paquetes = new List<Paquete>();
        _historial = new List<Acontecimiento>();
    }

    // Constructor horizontal
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
        _historial = new List<Acontecimiento>();
    }

    // Propiedades publicas
    public string CodigoEstablecimiento { get; private set; }
    public string Destinatario { get; private set; }
    public string CiudadDestino { get; private set; }
    public string Transportadora { get; private set; }
    public string EstadoActual { get { return _estadoActual; } set { _estadoActual = value; } }
    public ModalidadServicio Modalidad { get { return _modalidad; } set { _modalidad = value; } }
    public List<Paquete> Paquetes { get { return _paquetes; } }
    public List<Acontecimiento> Historial { get { return _historial; } }

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

    // Metodos de negocio
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

        foreach (Paquete p in _paquetes)
        {
            if (p.IdPaquete.ToLower() == paquete.IdPaquete.ToLower())
            {
                throw new NexoLogException($"El paquete con ID '{paquete.IdPaquete}' ya se encuentra registrado en el despacho {NumeroGuia}.");
            }
        }

        _paquetes.Add(paquete);
    }

    public double CalcularPesoTotal()
    {
        double pesoTotal = 0.0;
        foreach (Paquete paquete in _paquetes)
        {
            pesoTotal += paquete.Peso;
        }
        return pesoTotal;
    }

    public bool EsFacturable()
    {
        return _estadoActual.ToLower() == EstadoEntregado.ToLower();
    }

    public bool EvaluarCumplimiento()
    {
        if (_modalidad != null && _estadoActual.ToLower() == EstadoEntregado.ToLower())
        {
            return _modalidad.EvaluarCumplimiento(this);
        }
        return false;
    }

    public void ProcesarAcontecimiento(Acontecimiento evt)
    {
        if (evt == null)
        {
            throw new NexoLogException("No se puede procesar un acontecimiento nulo.");
        }

        ValidarNoFinalizado();

        if (evt.Tipo == Acontecimiento.TipoRec)
        {
            ProcesarRecepcion(evt);
        }
        else if (evt.Tipo == Acontecimiento.TipoRut)
        {
            ProcesarSalidaRuta(evt);
        }
        else if (evt.Tipo == Acontecimiento.TipoEnt)
        {
            ProcesarEntrega(evt);
        }
        else if (evt.Tipo == Acontecimiento.TipoDev)
        {
            ProcesarDevolucion(evt);
        }
        else
        {
            throw new NexoLogException($"Tipo de acontecimiento desconocido: '{evt.Tipo}'.");
        }
    }

    private void ValidarNoFinalizado()
    {
        if (_estadoActual.ToLower() == EstadoEntregado.ToLower() || _estadoActual.ToLower() == EstadoDevuelto.ToLower())
        {
            throw new NexoLogException($"El despacho ya se encuentra finalizado en estado '{_estadoActual}'.");
        }
    }

    private void ProcesarRecepcion(Acontecimiento evt)
    {
        if (_estadoActual.ToLower() != EstadoPendiente.ToLower())
        {
            throw new NexoLogException($"No se puede registrar recepcion en estado '{_estadoActual}'.");
        }

        foreach (Acontecimiento hito in _historial)
        {
            if (hito.Tipo == Acontecimiento.TipoRec)
            {
                throw new NexoLogException($"El despacho '{NumeroGuia}' ya cuenta con un acontecimiento de recepcion registrado.");
            }
        }

        _historial.Add(evt);
    }

    private void ProcesarSalidaRuta(Acontecimiento evt)
    {
        if (_estadoActual.ToLower() != EstadoPendiente.ToLower())
        {
            throw new NexoLogException($"No se puede pasar a ruta desde el estado '{_estadoActual}'.");
        }

        bool tieneRec = false;
        foreach (Acontecimiento hito in _historial)
        {
            if (hito.Tipo == Acontecimiento.TipoRec)
            {
                tieneRec = true;
                break;
            }
        }

        if (!tieneRec)
        {
            throw new NexoLogException("No se puede enviar a ruta sin un acontecimiento previo de recepcion.");
        }

        _estadoActual = EstadoEnDistribucion;
        _historial.Add(evt);
    }

    private void ProcesarEntrega(Acontecimiento evt)
    {
        if (_estadoActual.ToLower() != EstadoEnDistribucion.ToLower())
        {
            throw new NexoLogException($"No se puede registrar entrega si el despacho no esta en distribucion (estado actual: '{_estadoActual}').");
        }

        _estadoActual = EstadoEntregado;
        _historial.Add(evt);
    }

    private void ProcesarDevolucion(Acontecimiento evt)
    {
        if (_estadoActual.ToLower() != EstadoEnDistribucion.ToLower())
        {
            throw new NexoLogException($"No se puede registrar devolucion si el despacho no esta en distribucion (estado actual: '{_estadoActual}').");
        }

        _estadoActual = EstadoDevuelto;
        _historial.Add(evt);
    }

    public void RegistrarAcontecimiento(Acontecimiento acontecimiento)
    {
        ProcesarAcontecimiento(acontecimiento);
    }

    public string MostrarInformacion()
    {
        string modalidadNombre = _modalidad != null ? _modalidad.NombreModalidad : "Sin Asignar";
        return $"Guia: {NumeroGuia} | Modalidad: {modalidadNombre} | Destino: {CiudadDestino} ({Destinatario}) | Distancia: {DistanciaKm:F1} km | Estado: {EstadoActual} | Paquetes: {CantidadPaquetes} ({CalcularPesoTotal():F2} kg)";
    }

    public bool EsIdenticoA(Despacho candidato)
    {
        if (candidato == null)
        {
            return false;
        }
        if (CodigoEstablecimiento.ToLower() != candidato.CodigoEstablecimiento.ToLower())
        {
            return false;
        }
        if (Destinatario.ToLower() != candidato.Destinatario.ToLower())
        {
            return false;
        }
        if (CiudadDestino.ToLower() != candidato.CiudadDestino.ToLower())
        {
            return false;
        }
        if (Math.Abs(DistanciaKm - candidato.DistanciaKm) > 0.001)
        {
            return false;
        }
        if (Transportadora.ToLower() != candidato.Transportadora.ToLower())
        {
            return false;
        }
        if (CantidadPaquetes != candidato.CantidadPaquetes)
        {
            return false;
        }
        if (Math.Abs(CalcularPesoTotal() - candidato.CalcularPesoTotal()) > 0.001)
        {
            return false;
        }

        return true;
    }
}
