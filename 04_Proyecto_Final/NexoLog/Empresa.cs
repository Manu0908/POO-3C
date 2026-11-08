using NexoLog.Informes;

namespace NexoLog;

// Entidad raiz de dominio que coordina y encapsula las operaciones logisticas de NexoLog.
public class Empresa
{
    private Dictionary<string, Establecimiento> _establecimientos;
    private Dictionary<string, Despacho> _despachosPorGuia;
    private List<RegistroInconsistencia> _inconsistencias;

    public Dictionary<string, Establecimiento>.ValueCollection Establecimientos { get { return _establecimientos.Values; } }
    public Dictionary<string, Despacho>.ValueCollection Despachos { get { return _despachosPorGuia.Values; } }
    public List<RegistroInconsistencia> Inconsistencias { get { return _inconsistencias; } }

    public Empresa()
    {
        _establecimientos = new Dictionary<string, Establecimiento>(StringComparer.OrdinalIgnoreCase);
        _despachosPorGuia = new Dictionary<string, Despacho>(StringComparer.OrdinalIgnoreCase);
        _inconsistencias = new List<RegistroInconsistencia>();
    }

    public void Limpiar()
    {
        _establecimientos.Clear();
        _despachosPorGuia.Clear();
        _inconsistencias.Clear();
    }

    public void RegistrarEstablecimiento(Establecimiento nuevoEstablecimiento)
    {
        if (nuevoEstablecimiento == null)
        {
            return;
        }

        Establecimiento existente;
        if (_establecimientos.TryGetValue(nuevoEstablecimiento.Codigo, out existente))
        {
            if (nuevoEstablecimiento.TarifaAcordada != null)
            {
                existente.AsignarTarifa(nuevoEstablecimiento.TarifaAcordada);
            }
        }
        else
        {
            _establecimientos.Add(nuevoEstablecimiento.Codigo, nuevoEstablecimiento);
        }
    }

    public void RegistrarDespacho(Despacho despacho, string archivoOrigen)
    {
        if (despacho == null)
        {
            return;
        }

        string guia = despacho.NumeroGuia;
        Despacho existente;

        if (_despachosPorGuia.TryGetValue(guia, out existente))
        {
            bool esIdentico = existente.EsIdenticoA(despacho);
            string tipo = esIdentico ? RegistroInconsistencia.TipoDuplicado : RegistroInconsistencia.TipoConflictoIdentificacion;
            string motivo = esIdentico
                ? $"Despacho con guia '{guia}' duplicado identicamente en la fuente; se omite duplicado."
                : $"Conflicto de identificacion: La guia '{guia}' ya existe con informacion contradictoria.";
            _inconsistencias.Add(new RegistroInconsistencia(archivoOrigen, guia, tipo, motivo, DateTime.Now));
            return;
        }

        string codEst = despacho.CodigoEstablecimiento;
        Establecimiento est;
        if (!_establecimientos.TryGetValue(codEst, out est))
        {
            est = new Establecimiento(codEst, $"Comercio {codEst}");
            _establecimientos.Add(codEst, est);
        }

        est.AgregarDespacho(despacho);
        _despachosPorGuia.Add(guia, despacho);
    }

    public void RegistrarSeguimientos(List<Acontecimiento> acontecimientos, string archivoOrigen)
    {
        Dictionary<string, List<Acontecimiento>> agrupados = new Dictionary<string, List<Acontecimiento>>(StringComparer.OrdinalIgnoreCase);
        foreach (Acontecimiento evt in acontecimientos)
        {
            if (!agrupados.ContainsKey(evt.NumeroGuia))
            {
                agrupados[evt.NumeroGuia] = new List<Acontecimiento>();
            }
            agrupados[evt.NumeroGuia].Add(evt);
        }

        foreach (KeyValuePair<string, List<Acontecimiento>> par in agrupados)
        {
            Despacho despacho;
            if (!_despachosPorGuia.TryGetValue(par.Key, out despacho))
            {
                foreach (Acontecimiento e in par.Value)
                {
                    _inconsistencias.Add(new RegistroInconsistencia(
                        archivoOrigen,
                        e.IdAcontecimiento,
                        RegistroInconsistencia.TipoReferenciaInexistente,
                        $"El acontecimiento '{e.IdAcontecimiento}' referencia a una guia inexistente: '{par.Key}'.",
                        DateTime.Now));
                }
                continue;
            }

            ProcesarEventosDeGuia(despacho, par.Value, archivoOrigen);
        }
    }

    private void ProcesarEventosDeGuia(Despacho despacho, List<Acontecimiento> eventos, string archivoOrigen)
    {
        Dictionary<string, Acontecimiento> vistos = new Dictionary<string, Acontecimiento>(StringComparer.OrdinalIgnoreCase);
        List<Acontecimiento> validos = new List<Acontecimiento>();

        foreach (Acontecimiento evt in eventos)
        {
            Acontecimiento prev;
            if (vistos.TryGetValue(evt.IdAcontecimiento, out prev))
            {
                bool identico = prev.Tipo == evt.Tipo && prev.FechaHora == evt.FechaHora;
                string tipo = identico ? RegistroInconsistencia.TipoDuplicado : RegistroInconsistencia.TipoConflictoIdentificacion;
                string motivo = identico
                    ? $"Acontecimiento '{evt.IdAcontecimiento}' duplicado identicamente en la guia '{despacho.NumeroGuia}'."
                    : $"Conflicto en acontecimiento '{evt.IdAcontecimiento}' de la guia '{despacho.NumeroGuia}': mismo ID con datos contradictorios.";
                _inconsistencias.Add(new RegistroInconsistencia(archivoOrigen, evt.IdAcontecimiento, tipo, motivo, DateTime.Now));
                continue;
            }
            vistos.Add(evt.IdAcontecimiento, evt);
            validos.Add(evt);
        }

        List<Acontecimiento> cronologicos = validos.OrderBy(e => e.FechaHora).ToList();
        foreach (Acontecimiento evt in cronologicos)
        {
            try
            {
                despacho.ProcesarAcontecimiento(evt);
            }
            catch (NexoLogException ex)
            {
                _inconsistencias.Add(new RegistroInconsistencia(
                    archivoOrigen,
                    evt.IdAcontecimiento,
                    RegistroInconsistencia.TipoAcontecimientoIncompatible,
                    $"Incompatibilidad de secuencia en la guia '{despacho.NumeroGuia}': {ex.Message}",
                    DateTime.Now));
            }
        }
    }

    public InformeOperativo GenerarInformeOperativo()
    {
        return InformeOperativo.Crear(_despachosPorGuia.Values, _establecimientos.Values);
    }

    public InformeFinanciero GenerarInformeFinanciero()
    {
        return InformeFinanciero.Crear(_establecimientos.Values, _despachosPorGuia.Values);
    }

    public InformeInconsistencias GenerarInformeInconsistencias()
    {
        return InformeInconsistencias.Crear(_inconsistencias);
    }

    public Despacho ObtenerDespacho(string guia)
    {
        Despacho d;
        _despachosPorGuia.TryGetValue(guia, out d);
        return d;
    }

    public Establecimiento ObtenerEstablecimiento(string codigo)
    {
        Establecimiento e;
        _establecimientos.TryGetValue(codigo, out e);
        return e;
    }
}
