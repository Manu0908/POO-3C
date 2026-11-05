using NexoLog.DTOs;
using NexoLog.Informes;
using NexoLog.Lectores;
using NexoLog.Tarifas;
using NexoLog.Servicios;

namespace NexoLog;

// Entidad principal que coordina la carga, integracion multifuente y generacion de informes en NexoLog.
public class Empresa
{
    // Campos privados
    private Dictionary<string, Establecimiento> _establecimientos;
    private Dictionary<string, Despacho> _despachosPorGuia;
    private List<RegistroInconsistencia> _inconsistencias;

    private List<AcuerdoDto> _acuerdosCargados;
    private List<Despacho> _pedidosCaribeCargados;
    private List<Despacho> _pedidosAndinoCargados;
    private List<Acontecimiento> _seguimientosNorteCargados;
    private List<Acontecimiento> _seguimientosAndinaCargados;

    private bool _archivosCargados;
    private bool _informacionIntegrada;

    // Propiedades calculadas / derivadas
    public bool ArchivosCargados { get { return _archivosCargados; } }
    public bool InformacionIntegrada { get { return _informacionIntegrada; } }
    public Dictionary<string, Establecimiento>.ValueCollection Establecimientos { get { return _establecimientos.Values; } }
    public Dictionary<string, Despacho>.ValueCollection Despachos { get { return _despachosPorGuia.Values; } }
    public List<RegistroInconsistencia> Inconsistencias { get { return _inconsistencias; } }
    public int TotalAcuerdosCargados { get { return _acuerdosCargados.Count; } }
    public int TotalDespachosPreliminares { get { return _pedidosCaribeCargados.Count + _pedidosAndinoCargados.Count; } }
    public int TotalSeguimientosPreliminares { get { return _seguimientosNorteCargados.Count + _seguimientosAndinaCargados.Count; } }

    // Constructor horizontal
    public Empresa()
    {
        _establecimientos = new Dictionary<string, Establecimiento>(StringComparer.OrdinalIgnoreCase);
        _despachosPorGuia = new Dictionary<string, Despacho>(StringComparer.OrdinalIgnoreCase);
        _inconsistencias = new List<RegistroInconsistencia>();
        _acuerdosCargados = new List<AcuerdoDto>();
        _pedidosCaribeCargados = new List<Despacho>();
        _pedidosAndinoCargados = new List<Despacho>();
        _seguimientosNorteCargados = new List<Acontecimiento>();
        _seguimientosAndinaCargados = new List<Acontecimiento>();
    }

    // Carga resiliente de los 5 archivos del sistema
    public bool CargarArchivos(string carpetaDatos)
    {
        if (!Directory.Exists(carpetaDatos))
        {
            return false;
        }

        LimpiarEstado();

        string rutaAcuerdos = Path.Combine(carpetaDatos, "acuerdos.json");
        string rutaCaribe = Path.Combine(carpetaDatos, "pedidos_caribe.json");
        string rutaAndino = Path.Combine(carpetaDatos, "pedidos_andino.xml");
        string rutaNorte = Path.Combine(carpetaDatos, "seguimiento_norte.json");
        string rutaAndina = Path.Combine(carpetaDatos, "seguimiento_andina.xml");

        ILectorArchivo<AcuerdoDto> lectorAcuerdos = CreadorLectores.CrearLectorAcuerdosJson();
        ILectorArchivo<Despacho> lectorCaribe = CreadorLectores.CrearLectorPedidosJson();
        ILectorArchivo<Despacho> lectorAndino = CreadorLectores.CrearLectorPedidosXml();
        ILectorArchivo<Acontecimiento> lectorNorte = CreadorLectores.CrearLectorSeguimientosJson();
        ILectorArchivo<Acontecimiento> lectorAndina = CreadorLectores.CrearLectorSeguimientosXml();

        _acuerdosCargados = lectorAcuerdos.Leer(rutaAcuerdos, _inconsistencias);
        _pedidosCaribeCargados = lectorCaribe.Leer(rutaCaribe, _inconsistencias);
        _pedidosAndinoCargados = lectorAndino.Leer(rutaAndino, _inconsistencias);
        _seguimientosNorteCargados = lectorNorte.Leer(rutaNorte, _inconsistencias);
        _seguimientosAndinaCargados = lectorAndina.Leer(rutaAndina, _inconsistencias);

        _archivosCargados = true;
        _informacionIntegrada = false;
        return true;
    }

    // Integracion general aplicando reglas de negocio
    public void Integrar()
    {
        _establecimientos.Clear();
        _despachosPorGuia.Clear();

        IntegrarAcuerdos(_acuerdosCargados, "acuerdos.json");
        IntegrarLoteDespachos(_pedidosCaribeCargados, "pedidos_caribe.json");
        IntegrarLoteDespachos(_pedidosAndinoCargados, "pedidos_andino.xml");
        IntegrarSeguimientos(_seguimientosNorteCargados, "seguimiento_norte.json");
        IntegrarSeguimientos(_seguimientosAndinaCargados, "seguimiento_andina.xml");

        _informacionIntegrada = true;
    }

    private void LimpiarEstado()
    {
        _establecimientos.Clear();
        _despachosPorGuia.Clear();
        _inconsistencias.Clear();
        _acuerdosCargados.Clear();
        _pedidosCaribeCargados.Clear();
        _pedidosAndinoCargados.Clear();
        _seguimientosNorteCargados.Clear();
        _seguimientosAndinaCargados.Clear();
        _archivosCargados = false;
        _informacionIntegrada = false;
    }

    private void IntegrarAcuerdos(List<AcuerdoDto> acuerdos, string nombreArchivo)
    {
        foreach (AcuerdoDto dto in acuerdos)
        {
            string codigo = dto.CodigoComercio.Trim();
            string nombre = dto.NombreComercio.Trim();
            decimal baseTarifa = dto.TarifaBase;
            string tipoTarifa = (dto.TipoTarifa ?? string.Empty).Trim().ToUpperInvariant();

            ITarifa tarifa;
            if (tipoTarifa == "PESO")
            {
                tarifa = new TarifaPorPeso(baseTarifa);
            }
            else if (tipoTarifa == "DISTANCIA")
            {
                tarifa = new TarifaPorDistancia(baseTarifa);
            }
            else if (tipoTarifa == "FIJA")
            {
                tarifa = new TarifaFija(baseTarifa);
            }
            else
            {
                _inconsistencias.Add(new RegistroInconsistencia(
                    nombreArchivo,
                    codigo,
                    RegistroInconsistencia.TipoDatoInvalido,
                    $"Modalidad de tarifa desconocida '{dto.TipoTarifa}' para el comercio {codigo}.",
                    DateTime.Now));
                continue;
            }

            if (_establecimientos.TryGetValue(codigo, out Establecimiento establecimientoExistente))
            {
                establecimientoExistente.AsignarTarifa(tarifa);
            }
            else
            {
                Establecimiento nuevo = new Establecimiento(codigo, nombre, tarifa);
                _establecimientos.Add(codigo, nuevo);
            }
        }
    }

    private void IntegrarLoteDespachos(List<Despacho> despachos, string nombreArchivo)
    {
        foreach (Despacho despacho in despachos)
        {
            string guia = despacho.NumeroGuia;

            if (_despachosPorGuia.TryGetValue(guia, out Despacho despachoExistente))
            {
                bool esIdentico = EsDespachoIdentico(despachoExistente, despacho);
                if (esIdentico)
                {
                    _inconsistencias.Add(new RegistroInconsistencia(
                        nombreArchivo,
                        guia,
                        RegistroInconsistencia.TipoDuplicado,
                        $"Despacho con guia '{guia}' duplicado identicamente en la fuente; se omite duplicado.",
                        DateTime.Now));
                }
                else
                {
                    _inconsistencias.Add(new RegistroInconsistencia(
                        nombreArchivo,
                        guia,
                        RegistroInconsistencia.TipoConflictoIdentificacion,
                        $"Conflicto de identificacion: La guia '{guia}' ya existe con informacion contradictoria.",
                        DateTime.Now));
                }
                continue;
            }

            string codigoEst = despacho.CodigoEstablecimiento;
            if (!_establecimientos.TryGetValue(codigoEst, out Establecimiento establecimiento))
            {
                establecimiento = new Establecimiento(codigoEst, $"Comercio {codigoEst}");
                _establecimientos.Add(codigoEst, establecimiento);
            }

            establecimiento.AgregarDespacho(despacho);
            _despachosPorGuia.Add(guia, despacho);
        }
    }

    private void IntegrarSeguimientos(List<Acontecimiento> acontecimientos, string nombreArchivo)
    {
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

        foreach (KeyValuePair<string, List<Acontecimiento>> par in agrupadosPorGuia)
        {
            string guia = par.Key;
            List<Acontecimiento> listaEventos = par.Value;

            if (!_despachosPorGuia.TryGetValue(guia, out Despacho despacho))
            {
                foreach (Acontecimiento e in listaEventos)
                {
                    _inconsistencias.Add(new RegistroInconsistencia(
                        nombreArchivo,
                        e.IdAcontecimiento,
                        RegistroInconsistencia.TipoReferenciaInexistente,
                        $"El acontecimiento '{e.IdAcontecimiento}' referencia a una guia inexistente: '{guia}'.",
                        DateTime.Now));
                }
                continue;
            }

            ProcesarEventosDeGuia(despacho, listaEventos, nombreArchivo);
        }
    }

    private void ProcesarEventosDeGuia(Despacho despacho, List<Acontecimiento> listaEventos, string nombreArchivo)
    {
        string guia = despacho.NumeroGuia;
        Dictionary<string, Acontecimiento> eventosVistos = new Dictionary<string, Acontecimiento>(StringComparer.OrdinalIgnoreCase);
        List<Acontecimiento> eventosAProcesar = new List<Acontecimiento>();

        foreach (Acontecimiento evt in listaEventos)
        {
            string idEvt = evt.IdAcontecimiento;
            if (eventosVistos.TryGetValue(idEvt, out Acontecimiento evtPrevio))
            {
                if (evtPrevio.Tipo == evt.Tipo && evtPrevio.FechaHora == evt.FechaHora)
                {
                    _inconsistencias.Add(new RegistroInconsistencia(
                        nombreArchivo,
                        idEvt,
                        RegistroInconsistencia.TipoDuplicado,
                        $"Acontecimiento '{idEvt}' duplicado identicamente en la guia '{guia}'.",
                        DateTime.Now));
                }
                else
                {
                    _inconsistencias.Add(new RegistroInconsistencia(
                        nombreArchivo,
                        idEvt,
                        RegistroInconsistencia.TipoConflictoIdentificacion,
                        $"Conflicto en acontecimiento '{idEvt}' de la guia '{guia}': mismo ID con datos contradictorios.",
                        DateTime.Now));
                }
                continue;
            }

            eventosVistos.Add(idEvt, evt);
            eventosAProcesar.Add(evt);
        }

        // Orden cronologico ascendente obligatorio antes de procesar acontecimientos
        List<Acontecimiento> cronologicos = eventosAProcesar.OrderBy(e => e.FechaHora).ToList();

        foreach (Acontecimiento evt in cronologicos)
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
                    RegistroInconsistencia.TipoAcontecimientoIncompatible,
                    $"Incompatibilidad de secuencia en la guia '{guia}': {ex.Message}",
                    DateTime.Now));
            }
        }
    }

    private bool EsDespachoIdentico(Despacho original, Despacho candidato)
    {
        if (original.CodigoEstablecimiento.ToLower() != candidato.CodigoEstablecimiento.ToLower())
            return false;
        if (original.Destinatario.ToLower() != candidato.Destinatario.ToLower())
            return false;
        if (original.CiudadDestino.ToLower() != candidato.CiudadDestino.ToLower())
            return false;
        if (Math.Abs(original.DistanciaKm - candidato.DistanciaKm) > 0.001)
            return false;
        if (original.Transportadora.ToLower() != candidato.Transportadora.ToLower())
            return false;
        if (original.CantidadPaquetes != candidato.CantidadPaquetes)
            return false;
        if (Math.Abs(original.CalcularPesoTotal() - candidato.CalcularPesoTotal()) > 0.001)
            return false;

        return true;
    }

    // Generacion de Informe Operativo
    public InformeOperativo GenerarInformeOperativo()
    {
        InformeOperativo informe = new InformeOperativo();
        List<Despacho> todosDespachos = _despachosPorGuia.Values.ToList();

        informe.TotalDespachos = todosDespachos.Count;

        int entregados = 0;
        int devueltos = 0;
        int pendientes = 0;
        int enDistribucion = 0;
        int totalPaquetes = 0;
        double pesoTotal = 0.0;
        int cumplidos = 0;
        int incumplidos = 0;

        foreach (Despacho d in todosDespachos)
        {
            totalPaquetes += d.CantidadPaquetes;
            pesoTotal += d.CalcularPesoTotal();

            if (d.EstadoActual.ToLower() == Despacho.EstadoEntregado.ToLower())
            {
                entregados++;
                if (d.EvaluarCumplimiento())
                {
                    cumplidos++;
                }
                else
                {
                    incumplidos++;
                }
            }
            else if (d.EstadoActual.ToLower() == Despacho.EstadoDevuelto.ToLower())
            {
                devueltos++;
            }
            else if (d.EstadoActual.ToLower() == Despacho.EstadoEnDistribucion.ToLower())
            {
                enDistribucion++;
            }
            else
            {
                pendientes++;
            }
        }

        informe.TotalEntregados = entregados;
        informe.TotalDevueltos = devueltos;
        informe.TotalPendientes = pendientes;
        informe.TotalEnDistribucion = enDistribucion;
        informe.TotalPaquetes = totalPaquetes;
        informe.PesoTotalKg = pesoTotal;
        informe.CumplidosSLA = cumplidos;
        informe.IncumplidosSLA = incumplidos;

        // Porcentaje SLA exclusivamente sobre despachos entregados
        informe.PorcentajeCumplimientoSobreEntregados = entregados > 0 ? (cumplidos * 100.0) / entregados : 0.0;

        // Desglose por comercio
        foreach (Establecimiento est in _establecimientos.Values)
        {
            ResumenComercioOperativo com = new ResumenComercioOperativo();
            com.CodigoComercio = est.Codigo;
            com.NombreComercio = est.NombreComercial;
            com.TotalDespachos = est.CantidadDespachos;

            int entC = 0;
            int devC = 0;
            int penC = 0;
            int distC = 0;
            int cumpC = 0;
            int incumpC = 0;

            foreach (Despacho d in est.Despachos)
            {
                if (d.EstadoActual.ToLower() == Despacho.EstadoEntregado.ToLower())
                {
                    entC++;
                    if (d.EvaluarCumplimiento()) cumpC++;
                    else incumpC++;
                }
                else if (d.EstadoActual.ToLower() == Despacho.EstadoDevuelto.ToLower())
                {
                    devC++;
                }
                else if (d.EstadoActual.ToLower() == Despacho.EstadoEnDistribucion.ToLower())
                {
                    distC++;
                }
                else
                {
                    penC++;
                }
            }

            com.Entregados = entC;
            com.Devueltos = devC;
            com.Pendientes = penC;
            com.EnDistribucion = distC;
            com.CumplidosSLA = cumpC;
            com.IncumplidosSLA = incumpC;
            com.PorcentajeCumplimiento = entC > 0 ? (cumpC * 100.0) / entC : 0.0;

            informe.DesgloseComercios.Add(com);
        }

        // Desglose por modalidad
        Dictionary<string, List<Despacho>> porModalidad = new Dictionary<string, List<Despacho>>(StringComparer.OrdinalIgnoreCase);
        foreach (Despacho d in todosDespachos)
        {
            string nombreMod = d.Modalidad != null ? d.Modalidad.NombreModalidad : "Sin Asignar";
            if (!porModalidad.ContainsKey(nombreMod))
            {
                porModalidad[nombreMod] = new List<Despacho>();
            }
            porModalidad[nombreMod].Add(d);
        }

        foreach (KeyValuePair<string, List<Despacho>> grupo in porModalidad)
        {
            ResumenModalidadOperativo mod = new ResumenModalidadOperativo();
            mod.Modalidad = grupo.Key;
            mod.TotalDespachos = grupo.Value.Count;

            int entM = 0;
            int cumpM = 0;
            int incumpM = 0;

            foreach (Despacho d in grupo.Value)
            {
                if (d.EstadoActual.ToLower() == Despacho.EstadoEntregado.ToLower())
                {
                    entM++;
                    if (d.EvaluarCumplimiento()) cumpM++;
                    else incumpM++;
                }
            }

            mod.Entregados = entM;
            mod.CumplidosSLA = cumpM;
            mod.IncumplidosSLA = incumpM;
            mod.PorcentajeCumplimiento = entM > 0 ? (cumpM * 100.0) / entM : 0.0;

            informe.DesgloseModalidades.Add(mod);
        }

        return informe;
    }

    // Generacion de Informe Financiero
    public InformeFinanciero GenerarInformeFinanciero()
    {
        InformeFinanciero informe = new InformeFinanciero();
        decimal granTotal = 0m;
        int noValorados = 0;

        foreach (Establecimiento est in _establecimientos.Values)
        {
            ItemFinancieroComercio item = new ItemFinancieroComercio();
            item.CodigoComercio = est.Codigo;
            item.NombreComercio = est.NombreComercial;

            string nombreTarifa = est.TarifaAcordada != null ? est.TarifaAcordada.ObtenerTipoTarifa() : "Sin Tarifa Asignada";
            item.ModalidadTarifa = nombreTarifa;

            decimal tarifaBase = 0m;
            if (est.TarifaAcordada is TarifaPorPeso tPeso)
            {
                tarifaBase = tPeso.ValorPorKilogramo;
            }
            else if (est.TarifaAcordada is TarifaPorDistancia tDist)
            {
                tarifaBase = tDist.ValorPorKilometro;
            }
            else if (est.TarifaAcordada is TarifaFija tFija)
            {
                tarifaBase = tFija.ValorFijo;
            }

            item.TarifaBase = tarifaBase;
            item.DespachosTotales = est.CantidadDespachos;

            int facturables = 0;
            foreach (Despacho d in est.Despachos)
            {
                if (d.EsFacturable())
                {
                    facturables++;
                }
            }

            item.DespachosFacturables = facturables;
            decimal totalComercio = est.CalcularTotalFacturable();
            item.TotalFacturado = totalComercio;
            granTotal += totalComercio;

            informe.DesgloseComercios.Add(item);

            if (!informe.CobrosPorModalidadTarifa.ContainsKey(nombreTarifa))
            {
                informe.CobrosPorModalidadTarifa[nombreTarifa] = 0m;
            }
            informe.CobrosPorModalidadTarifa[nombreTarifa] += totalComercio;
        }

        foreach (Despacho d in _despachosPorGuia.Values)
        {
            if (d.EsFacturable())
            {
                _establecimientos.TryGetValue(d.CodigoEstablecimiento, out Establecimiento est);
                if (est == null || est.TarifaAcordada == null)
                {
                    noValorados++;
                }
            }
        }

        informe.GranTotalFacturado = granTotal;
        informe.DespachosEntregadosNoValorados = noValorados;

        return informe;
    }

    // Generacion de Informe de Inconsistencias
    public InformeInconsistencias GenerarInformeInconsistencias()
    {
        InformeInconsistencias informe = new InformeInconsistencias();
        informe.TotalInconsistencias = _inconsistencias.Count;
        informe.Listado = _inconsistencias;

        foreach (RegistroInconsistencia inc in _inconsistencias)
        {
            string tipo = inc.Tipo ?? "Desconocido";
            if (!informe.InconsistenciasPorTipo.ContainsKey(tipo))
            {
                informe.InconsistenciasPorTipo[tipo] = 0;
            }
            informe.InconsistenciasPorTipo[tipo]++;

            string archivo = inc.ArchivoOrigen ?? "Desconocido";
            if (!informe.InconsistenciasPorArchivo.ContainsKey(archivo))
            {
                informe.InconsistenciasPorArchivo[archivo] = 0;
            }
            informe.InconsistenciasPorArchivo[archivo]++;
        }

        return informe;
    }

    // Consultas individuales
    public Despacho ObtenerDespacho(string guia)
    {
        _despachosPorGuia.TryGetValue(guia, out Despacho d);
        return d;
    }

    public Establecimiento ObtenerEstablecimiento(string codigo)
    {
        _establecimientos.TryGetValue(codigo, out Establecimiento e);
        return e;
    }
}
