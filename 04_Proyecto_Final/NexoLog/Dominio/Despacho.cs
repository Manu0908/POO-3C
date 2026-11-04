using NexoLog.Dominio.Modalidades;

namespace NexoLog.Dominio;

// Representa una orden de despacho o envío logístico gestionado por la compañía.
public class Despacho
{
    public const string EstadoPendiente = "Pendiente";
    public const string EstadoEnDistribucion = "EnDistribucion";
    public const string EstadoEntregado = "Entregado";
    public const string EstadoDevuelto = "Devuelto";

    private string _numeroGuia;
    private string _codigoEstablecimiento;
    private string _destinatario;
    private string _ciudadDestino;
    private double _distanciaKm;
    private string _transportadora;
    private string _estadoActual;
    private ModalidadServicio _modalidad;
    private List<Paquete> _paquetes;
    private List<Acontecimiento> _historial;

    public string NumeroGuia
    {
        get { return _numeroGuia; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NexoLogException("El número de guía es obligatorio.");
            }
            _numeroGuia = value.Trim();
        }
    }

    public string CodigoEstablecimiento
    {
        get { return _codigoEstablecimiento; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NexoLogException("El código de establecimiento remitente es obligatorio.");
            }
            _codigoEstablecimiento = value.Trim();
        }
    }

    public string Destinatario
    {
        get { return _destinatario; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NexoLogException("El nombre del destinatario es obligatorio.");
            }
            _destinatario = value.Trim();
        }
    }

    public string CiudadDestino
    {
        get { return _ciudadDestino; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NexoLogException("La ciudad de destino es obligatoria.");
            }
            _ciudadDestino = value.Trim();
        }
    }

    public double DistanciaKm
    {
        get { return _distanciaKm; }
        private set
        {
            if (value <= 0)
            {
                throw new NexoLogException("La distancia en kilómetros debe ser estrictamente mayor a 0.");
            }
            _distanciaKm = value;
        }
    }

    public string Transportadora
    {
        get { return _transportadora; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NexoLogException("La empresa transportadora es obligatoria.");
            }
            _transportadora = value.Trim();
        }
    }

    public string EstadoActual
    {
        get { return _estadoActual; }
        set { _estadoActual = value; }
    }

    public ModalidadServicio Modalidad
    {
        get { return _modalidad; }
        set { _modalidad = value; }
    }

    public IReadOnlyList<Paquete> Paquetes
    {
        get { return _paquetes.AsReadOnly(); }
    }

    public IReadOnlyList<Acontecimiento> Historial
    {
        get { return _historial.AsReadOnly(); }
    }

    public int CantidadPaquetes
    {
        get { return _paquetes.Count; }
    }

    public Despacho(
        string numeroGuia,
        string codigoEstablecimiento,
        string destinatario,
        string ciudadDestino,
        double distanciaKm,
        string transportadora)
    {
        NumeroGuia = numeroGuia;
        CodigoEstablecimiento = codigoEstablecimiento;
        Destinatario = destinatario;
        CiudadDestino = ciudadDestino;
        DistanciaKm = distanciaKm;
        Transportadora = transportadora;

        // Estado inicial obligatorio según reglas de negocio
        _estadoActual = EstadoPendiente;
        _modalidad = null;
        _paquetes = new List<Paquete>();
        _historial = new List<Acontecimiento>();
    }

    // Asigna explícitamente la modalidad de servicio logístico al despacho
    public void AsignarModalidad(ModalidadServicio modalidad)
    {
        _modalidad = modalidad;
    }

    // Agrega un paquete al despacho verificando que no sea nulo ni esté duplicado
    public void AgregarPaquete(Paquete paquete)
    {
        if (paquete == null)
        {
            throw new NexoLogException("No se puede agregar un paquete nulo al despacho.");
        }

        foreach (Paquete p in _paquetes)
        {
            if (string.Equals(p.IdPaquete, paquete.IdPaquete, StringComparison.OrdinalIgnoreCase))
            {
                throw new NexoLogException($"El paquete con ID '{paquete.IdPaquete}' ya se encuentra registrado en el despacho {NumeroGuia}.");
            }
        }

        _paquetes.Add(paquete);
    }

    // Suma el peso de todos los paquetes asociados a este despacho
    public double CalcularPesoTotal()
    {
        double pesoTotal = 0.0;
        foreach (Paquete paquete in _paquetes)
        {
            pesoTotal += paquete.Peso;
        }
        return pesoTotal;
    }

    // Determina si el despacho puede ser objeto de cobro (solo entregados)
    public bool EsFacturable()
    {
        return string.Equals(_estadoActual, EstadoEntregado, StringComparison.OrdinalIgnoreCase);
    }

    // Evalúa si el despacho cumplió los acuerdos de nivel de servicio
    public bool EvaluarCumplimiento()
    {
        if (_modalidad != null && string.Equals(_estadoActual, EstadoEntregado, StringComparison.OrdinalIgnoreCase))
        {
            return _modalidad.EvaluarCumplimiento(this);
        }

        return false;
    }

    // Procesa un acontecimiento aplicando la máquina de estados estricta (REC -> RUT -> ENT/DEV)
    public void ProcesarAcontecimiento(Acontecimiento evt)
    {
        if (evt == null)
        {
            throw new NexoLogException("No se puede procesar un acontecimiento nulo.");
        }

        ValidarNoFinalizado();

        switch (evt.Tipo)
        {
            case Acontecimiento.TipoRec:
                ProcesarRecepcion(evt);
                break;
            case Acontecimiento.TipoRut:
                ProcesarSalidaRuta(evt);
                break;
            case Acontecimiento.TipoEnt:
                ProcesarEntrega(evt);
                break;
            case Acontecimiento.TipoDev:
                ProcesarDevolucion(evt);
                break;
            default:
                throw new NexoLogException($"Tipo de acontecimiento desconocido: '{evt.Tipo}'.");
        }
    }

    private void ValidarNoFinalizado()
    {
        if (string.Equals(_estadoActual, EstadoEntregado, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(_estadoActual, EstadoDevuelto, StringComparison.OrdinalIgnoreCase))
        {
            throw new NexoLogException($"El despacho ya se encuentra finalizado en estado '{_estadoActual}'.");
        }
    }

    private void ProcesarRecepcion(Acontecimiento evt)
    {
        if (!string.Equals(_estadoActual, EstadoPendiente, StringComparison.OrdinalIgnoreCase))
        {
            throw new NexoLogException($"No se puede registrar recepción (REC) en un despacho en estado '{_estadoActual}'.");
        }

        foreach (Acontecimiento hito in _historial)
        {
            if (string.Equals(hito.Tipo, Acontecimiento.TipoRec, StringComparison.OrdinalIgnoreCase))
            {
                throw new NexoLogException($"El despacho '{NumeroGuia}' ya cuenta con un acontecimiento de recepción (REC) registrado.");
            }
        }

        _historial.Add(evt);
    }

    private void ProcesarSalidaRuta(Acontecimiento evt)
    {
        if (!string.Equals(_estadoActual, EstadoPendiente, StringComparison.OrdinalIgnoreCase))
        {
            throw new NexoLogException($"No se puede pasar a ruta (RUT) desde el estado '{_estadoActual}'.");
        }

        bool tieneRec = false;
        foreach (Acontecimiento hito in _historial)
        {
            if (string.Equals(hito.Tipo, Acontecimiento.TipoRec, StringComparison.OrdinalIgnoreCase))
            {
                tieneRec = true;
                break;
            }
        }

        if (!tieneRec)
        {
            throw new NexoLogException("No se puede enviar a ruta (RUT) sin un acontecimiento previo de recepción (REC).");
        }

        _estadoActual = EstadoEnDistribucion;
        _historial.Add(evt);
    }

    private void ProcesarEntrega(Acontecimiento evt)
    {
        if (!string.Equals(_estadoActual, EstadoEnDistribucion, StringComparison.OrdinalIgnoreCase))
        {
            throw new NexoLogException($"No se puede registrar entrega (ENT) si el despacho no está en distribución (estado actual: '{_estadoActual}').");
        }

        _estadoActual = EstadoEntregado;
        _historial.Add(evt);
    }

    private void ProcesarDevolucion(Acontecimiento evt)
    {
        if (!string.Equals(_estadoActual, EstadoEnDistribucion, StringComparison.OrdinalIgnoreCase))
        {
            throw new NexoLogException($"No se puede registrar devolución (DEV) si el despacho no está en distribución (estado actual: '{_estadoActual}').");
        }

        _estadoActual = EstadoDevuelto;
        _historial.Add(evt);
    }

    public void RegistrarAcontecimiento(Acontecimiento acontecimiento)
    {
        ProcesarAcontecimiento(acontecimiento);
    }

    public override string ToString()
    {
        string modalidadNombre = _modalidad != null ? _modalidad.NombreModalidad : "Sin Asignar";
        return $"Guía: {NumeroGuia} | Modalidad: {modalidadNombre} | Destino: {CiudadDestino} ({Destinatario}) | Distancia: {DistanciaKm:F1} km | Estado: {EstadoActual} | Paquetes: {CantidadPaquetes} ({CalcularPesoTotal():F2} kg)";
    }
}
