using NexoLog.Dominio;
using NexoLog.Dominio.Inconsistencias;
using NexoLog.Dominio.Modalidades;
using NexoLog.Dominio.Tarifas;
using NexoLog.Infraestructura.Lectores.DTOs;

namespace NexoLog.Servicios;

/// <summary>
/// Servicio central de integración y consolidación de datos logísticos provenientes de múltiples fuentes.
/// </summary>
public class ServicioIntegracion
{
    private Dictionary<string, Establecimiento> _establecimientos;
    private Dictionary<string, Despacho> _despachosPorGuia;
    private List<RegistroInconsistencia> _inconsistencias;

    public IReadOnlyCollection<Establecimiento> Establecimientos
    {
        get { return _establecimientos.Values; }
    }

    public IReadOnlyCollection<Despacho> Despachos
    {
        get { return _despachosPorGuia.Values; }
    }

    public IReadOnlyList<RegistroInconsistencia> Inconsistencias
    {
        get { return _inconsistencias.AsReadOnly(); }
    }

    public ServicioIntegracion()
    {
        _establecimientos = new Dictionary<string, Establecimiento>(StringComparer.OrdinalIgnoreCase);
        _despachosPorGuia = new Dictionary<string, Despacho>(StringComparer.OrdinalIgnoreCase);
        _inconsistencias = new List<RegistroInconsistencia>();
    }

    /// <summary>
    /// Limpia todas las estructuras internas para reiniciar una nueva sesión de integración.
    /// </summary>
    public void Reiniciar()
    {
        _establecimientos.Clear();
        _despachosPorGuia.Clear();
        _inconsistencias.Clear();
    }

    /// <summary>
    /// Registra directamente una inconsistencia capturada externamente (por ejemplo, durante la lectura).
    /// </summary>
    public void AgregarInconsistencia(RegistroInconsistencia inconsistencia)
    {
        if (inconsistencia != null)
        {
            _inconsistencias.Add(inconsistencia);
        }
    }

    /// <summary>
    /// Registra en lote inconsistencias capturadas durante fases previas.
    /// </summary>
    public void AgregarInconsistencias(IEnumerable<RegistroInconsistencia> lista)
    {
        if (lista != null)
        {
            foreach (RegistroInconsistencia inc in lista)
            {
                if (inc != null)
                {
                    _inconsistencias.Add(inc);
                }
            }
        }
    }

    /// <summary>
    /// Integra los acuerdos comerciales pactados con los comercios remitentes.
    /// </summary>
    public void IntegrarAcuerdos(List<AcuerdoDto> acuerdos, string nombreArchivo)
    {
        if (acuerdos == null)
        {
            return;
        }

        foreach (AcuerdoDto dto in acuerdos)
        {
            string codigo = dto.CodigoComercio.Trim();
            string nombre = dto.NombreComercio.Trim();
            decimal baseTarifa = dto.TarifaBase;
            string tipoTarifa = dto.TipoTarifa.Trim().ToUpperInvariant();

            IEstrategiaCobro estrategia;
            if (tipoTarifa == "PESO")
            {
                estrategia = new TarifaPesoStrategy(baseTarifa);
            }
            else if (tipoTarifa == "DISTANCIA")
            {
                estrategia = new TarifaDistanciaStrategy(baseTarifa);
            }
            else if (tipoTarifa == "FIJA")
            {
                estrategia = new TarifaFijaStrategy(baseTarifa);
            }
            else
            {
                _inconsistencias.Add(new RegistroInconsistencia(
                    nombreArchivo,
                    codigo,
                    TipoInconsistencia.DatoInvalido,
                    $"Modalidad de tarifa desconocida '{dto.TipoTarifa}' para el comercio {codigo}.",
                    DateTime.Now));
                continue;
            }

            if (_establecimientos.TryGetValue(codigo, out Establecimiento establecimientoExistente))
            {
                establecimientoExistente.AsignarTarifa(estrategia);
            }
            else
            {
                Establecimiento nuevoEstablecimiento = new Establecimiento(codigo, nombre, estrategia);
                _establecimientos.Add(codigo, nuevoEstablecimiento);
            }
        }
    }

    /// <summary>
    /// Integra solicitudes de despacho validando duplicidad exacta y conflictos de identificación.
    /// </summary>
    public void IntegrarDespachos(List<Despacho> despachos, string nombreArchivo)
    {
        if (despachos == null)
        {
            return;
        }

        foreach (Despacho despacho in despachos)
        {
            string guia = despacho.NumeroGuia;

            if (_despachosPorGuia.TryGetValue(guia, out Despacho despachoExistente))
            {
                // Evaluar si es una duplicación idéntica o un conflicto de identificación
                bool esIdentico = EsDespachoIdentico(despachoExistente, despacho);

                if (esIdentico)
                {
                    _inconsistencias.Add(new RegistroInconsistencia(
                        nombreArchivo,
                        guia,
                        TipoInconsistencia.Duplicado,
                        $"Despacho con guía '{guia}' duplicado idénticamente en la fuente; se omite duplicado.",
                        DateTime.Now));
                }
                else
                {
                    _inconsistencias.Add(new RegistroInconsistencia(
                        nombreArchivo,
                        guia,
                        TipoInconsistencia.ConflictoIdentificacion,
                        $"Conflicto de identificación: La guía '{guia}' ya existe con información contradictoria.",
                        DateTime.Now));
                }

                // En ambos casos de choque, se preserva el primero y se ignora el repetido/conflictivo
                continue;
            }

            // Guía nueva: vincular al establecimiento y al catálogo global
            string codigoEstablecimiento = despacho.CodigoEstablecimiento;
            if (!_establecimientos.TryGetValue(codigoEstablecimiento, out Establecimiento establecimiento))
            {
                establecimiento = new Establecimiento(codigoEstablecimiento, $"Comercio {codigoEstablecimiento}");
                _establecimientos.Add(codigoEstablecimiento, establecimiento);
            }

            establecimiento.AgregarDespacho(despacho);
            _despachosPorGuia.Add(guia, despacho);
        }
    }

    /// <summary>
    /// Integra acontecimientos de seguimiento ordenándolos cronológicamente y verificando secuencias lógicas.
    /// </summary>
    public void IntegrarSeguimientos(List<Acontecimiento> acontecimientos, string nombreArchivo)
    {
        if (acontecimientos == null)
        {
            return;
        }

        // Agrupar los acontecimientos por guía
        Dictionary<string, List<Acontecimiento>> agrupadosPorGuia = new Dictionary<string, List<Acontecimiento>>(StringComparer.OrdinalIgnoreCase);

        foreach (Acontecimiento evt in acontecimientos)
        {
            string guia = evt.NumeroGuia;
            if (!agrupadosPorGuia.ContainsKey(guia))
            {
                agrupadosPorGuia[guia] = new List<Acontecimiento>();
            }
            agrupadosPorGuia[guia].Add(evt);
        }

        foreach (KeyValuePair<string, List<Acontecimiento>> grupo in agrupadosPorGuia)
        {
            string guia = grupo.Key;
            List<Acontecimiento> listaEventosGuia = grupo.Value;

            if (!_despachosPorGuia.TryGetValue(guia, out Despacho despacho))
            {
                foreach (Acontecimiento evtDesconocido in listaEventosGuia)
                {
                    _inconsistencias.Add(new RegistroInconsistencia(
                        nombreArchivo,
                        evtDesconocido.IdAcontecimiento,
                        TipoInconsistencia.ReferenciaInexistente,
                        $"El acontecimiento '{evtDesconocido.IdAcontecimiento}' referencia a una guía inexistente en el sistema: '{guia}'.",
                        DateTime.Now));
                }
                continue;
            }

            // Filtrar duplicados o conflictos a nivel de acontecimientos para la misma guía
            Dictionary<string, Acontecimiento> eventosVistos = new Dictionary<string, Acontecimiento>(StringComparer.OrdinalIgnoreCase);
            List<Acontecimiento> eventosAProcesar = new List<Acontecimiento>();

            foreach (Acontecimiento evt in listaEventosGuia)
            {
                string idEvt = evt.IdAcontecimiento;
                if (eventosVistos.TryGetValue(idEvt, out Acontecimiento evtRegistrado))
                {
                    if (evtRegistrado.Tipo == evt.Tipo && evtRegistrado.FechaHora == evt.FechaHora)
                    {
                        _inconsistencias.Add(new RegistroInconsistencia(
                            nombreArchivo,
                            idEvt,
                            TipoInconsistencia.Duplicado,
                            $"Acontecimiento '{idEvt}' duplicado idénticamente en la guía '{guia}'.",
                            DateTime.Now));
                    }
                    else
                    {
                        _inconsistencias.Add(new RegistroInconsistencia(
                            nombreArchivo,
                            idEvt,
                            TipoInconsistencia.ConflictoIdentificacion,
                            $"Conflicto en acontecimiento '{idEvt}' de la guía '{guia}': mismo ID con datos contradictorios.",
                            DateTime.Now));
                    }
                    continue;
                }

                eventosVistos.Add(idEvt, evt);
                eventosAProcesar.Add(evt);
            }

            // Ordenamiento cronológico obligatorio por FechaHora ascendente
            List<Acontecimiento> eventosOrdenados = eventosAProcesar.OrderBy(e => e.FechaHora).ToList();

            // Aplicación secuencial en la máquina de estados del despacho
            foreach (Acontecimiento evt in eventosOrdenados)
            {
                try
                {
                    despacho.ProcesarAcontecimiento(evt);
                }
                catch (NexoLogException ex)
                {
                    _inconsistencias.Add(new RegistroInconsistencia(
                        nombreArchivo,
                        evt.IdAcontecimiento,
                        TipoInconsistencia.AcontecimientoIncompatible,
                        $"Incompatibilidad de secuencia en la guía '{guia}': {ex.Message}",
                        DateTime.Now));
                }
            }
        }
    }

    /// <summary>
    /// Compara dos despachos para constatar si representan exactamente la misma información.
    /// </summary>
    private bool EsDespachoIdentico(Despacho original, Despacho candidato)
    {
        if (!string.Equals(original.CodigoEstablecimiento, candidato.CodigoEstablecimiento, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.Equals(original.Destinatario, candidato.Destinatario, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.Equals(original.CiudadDestino, candidato.CiudadDestino, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (Math.Abs(original.DistanciaKm - candidato.DistanciaKm) > 0.001)
        {
            return false;
        }

        if (!string.Equals(original.Transportadora, candidato.Transportadora, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (original.CantidadPaquetes != candidato.CantidadPaquetes)
        {
            return false;
        }

        if (Math.Abs(original.CalcularPesoTotal() - candidato.CalcularPesoTotal()) > 0.001)
        {
            return false;
        }

        return true;
    }
}
