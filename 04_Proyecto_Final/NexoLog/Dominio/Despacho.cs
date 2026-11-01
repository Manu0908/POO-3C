using NexoLog.Dominio.Modalidades;

namespace NexoLog.Dominio;

/// <summary>
/// Representa una orden de despacho o envío logístico gestionado por la compañía.
/// </summary>
public class Despacho
{
    private string _numeroGuia;
    private string _codigoEstablecimiento;
    private string _destinatario;
    private string _ciudadDestino;
    private double _distanciaKm;
    private string _transportadora;
    private EstadoDespacho _estadoActual;
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

    public EstadoDespacho EstadoActual
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
        
        // Estado inicial de todo despacho recién creado
        _estadoActual = EstadoDespacho.Pendiente;
        _modalidad = null;
        _paquetes = new List<Paquete>();
        _historial = new List<Acontecimiento>();
    }

    /// <summary>
    /// Asigna explícitamente la modalidad de servicio logístico (SLA) al despacho.
    /// </summary>
    public void AsignarModalidad(ModalidadServicio modalidad)
    {
        _modalidad = modalidad;
    }

    /// <summary>
    /// Agrega un paquete al despacho verificando que no sea nulo ni esté duplicado.
    /// </summary>
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

    /// <summary>
    /// Suma el peso de todos los paquetes asociados a este despacho.
    /// </summary>
    public double CalcularPesoTotal()
    {
        double pesoTotal = 0.0;
        foreach (Paquete paquete in _paquetes)
        {
            pesoTotal += paquete.Peso;
        }
        return pesoTotal;
    }

    /// <summary>
    /// Determina si el despacho puede ser objeto de liquidación y facturación (sólo si fue entregado).
    /// </summary>
    public bool EsFacturable()
    {
        return _estadoActual == EstadoDespacho.Entregado;
    }

    /// <summary>
    /// Evalúa si el despacho cumplió los acuerdos de nivel de servicio según su modalidad asignada.
    /// </summary>
    public bool EvaluarCumplimiento()
    {
        if (_modalidad != null && _estadoActual == EstadoDespacho.Entregado)
        {
            return _modalidad.EvaluarCumplimiento(this);
        }

        return false;
    }

    /// <summary>
    /// Procesa un acontecimiento aplicando la máquina de estados estricta (REC -> RUT -> ENT/DEV).
    /// </summary>
    public void ProcesarAcontecimiento(Acontecimiento evt)
    {
        if (evt == null)
        {
            throw new NexoLogException("No se puede procesar un acontecimiento nulo.");
        }

        // Si el despacho ya finalizó en estado terminal, no admite nuevos hitos
        if (_estadoActual == EstadoDespacho.Entregado || _estadoActual == EstadoDespacho.Devuelto)
        {
            throw new NexoLogException($"El despacho ya se encuentra finalizado en estado '{_estadoActual}'.");
        }

        switch (evt.Tipo)
        {
            case TipoAcontecimiento.REC:
                if (_estadoActual != EstadoDespacho.Pendiente)
                {
                    throw new NexoLogException($"No se puede registrar recepción (REC) en un despacho en estado '{_estadoActual}'.");
                }

                // Verificar si ya cuenta con un REC previo
                foreach (Acontecimiento hito in _historial)
                {
                    if (hito.Tipo == TipoAcontecimiento.REC)
                    {
                        throw new NexoLogException($"El despacho '{NumeroGuia}' ya cuenta con un acontecimiento de recepción (REC) registrado.");
                    }
                }

                _historial.Add(evt);
                break;

            case TipoAcontecimiento.RUT:
                if (_estadoActual != EstadoDespacho.Pendiente)
                {
                    throw new NexoLogException($"No se puede pasar a ruta (RUT) desde el estado '{_estadoActual}'.");
                }

                // Requiere obligatoriamente que exista un acontecimiento REC previo
                bool tieneRec = false;
                foreach (Acontecimiento hito in _historial)
                {
                    if (hito.Tipo == TipoAcontecimiento.REC)
                    {
                        tieneRec = true;
                        break;
                    }
                }

                if (!tieneRec)
                {
                    throw new NexoLogException("No se puede enviar a ruta (RUT) sin un acontecimiento previo de recepción (REC).");
                }

                _estadoActual = EstadoDespacho.EnDistribucion;
                _historial.Add(evt);
                break;

            case TipoAcontecimiento.ENT:
                if (_estadoActual != EstadoDespacho.EnDistribucion)
                {
                    throw new NexoLogException($"No se puede registrar entrega (ENT) si el despacho no está en distribución (estado actual: '{_estadoActual}').");
                }

                _estadoActual = EstadoDespacho.Entregado;
                _historial.Add(evt);
                break;

            case TipoAcontecimiento.DEV:
                if (_estadoActual != EstadoDespacho.EnDistribucion)
                {
                    throw new NexoLogException($"No se puede registrar devolución (DEV) si el despacho no está en distribución (estado actual: '{_estadoActual}').");
                }

                _estadoActual = EstadoDespacho.Devuelto;
                _historial.Add(evt);
                break;

            default:
                throw new NexoLogException($"Tipo de acontecimiento desconocido o no soportado: '{evt.Tipo}'.");
        }
    }

    /// <summary>
    /// Registra un acontecimiento en el historial de trazabilidad del despacho aplicando la máquina de estados.
    /// </summary>
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
