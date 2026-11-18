using NexoLog.DTOs;

namespace NexoLog;

public class Empresa
{
    private Dictionary<string, Establecimiento> _establecimientos;
    private Dictionary<string, Despacho> _despachosPorGuia;
    private List<RegistroInconsistencia> _inconsistencias;

    public List<Establecimiento> Establecimientos { get { return new List<Establecimiento>(_establecimientos.Values); } }
    public List<Despacho> Despachos { get { return new List<Despacho>(_despachosPorGuia.Values); } }
    public List<RegistroInconsistencia> Inconsistencias { get { return new List<RegistroInconsistencia>(_inconsistencias); } }

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

    public void RegistrarInconsistencia(RegistroInconsistencia inconsistencia)
    {
        if (inconsistencia == null)
        {
            throw new NexoLogException("No se puede registrar una inconsistencia nula.");
        }
        _inconsistencias.Add(inconsistencia);
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
            bool esIdentico = existente.TieneMismosDatosQue(despacho);
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
        IEnumerable<IGrouping<string, Acontecimiento>> agrupados = acontecimientos.GroupBy(e => e.NumeroGuia, StringComparer.OrdinalIgnoreCase);

        foreach (IGrouping<string, Acontecimiento> grupo in agrupados)
        {
            Despacho despacho;
            if (!_despachosPorGuia.TryGetValue(grupo.Key, out despacho))
            {
                foreach (Acontecimiento e in grupo)
                {
                    _inconsistencias.Add(new RegistroInconsistencia(
                        archivoOrigen,
                        e.IdAcontecimiento,
                        RegistroInconsistencia.TipoReferenciaInexistente,
                        $"El acontecimiento '{e.IdAcontecimiento}' referencia a una guia inexistente: '{grupo.Key}'.",
                        DateTime.Now));
                }
                continue;
            }

            RegistrarEventosEnDespacho(despacho, grupo.ToList(), archivoOrigen);
        }
    }

    private void RegistrarEventosEnDespacho(Despacho despacho, List<Acontecimiento> eventos, string archivoOrigen)
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
                despacho.AgregarAcontecimiento(evt);
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

    public InformeConsolidadoDto MapToDto()
    {
        ComercioOperativoDto resumenGlobal = ConstruirResumenOperativo("TOTAL", "Global", _despachosPorGuia.Values);
        InformeConsolidadoDto dto = new InformeConsolidadoDto
        {
            FechaGeneracion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            Sistema = "NexoLog S.A.S.",
            TotalDespachos = resumenGlobal.TotalDespachos,
            TotalEntregados = resumenGlobal.Entregados,
            TotalDevueltos = resumenGlobal.Devueltos,
            TotalPendientes = resumenGlobal.Pendientes,
            TotalEnDistribucion = resumenGlobal.EnDistribucion,
            TotalPaquetes = _despachosPorGuia.Values.Sum(d => d.CantidadPaquetes),
            PesoTotalKg = _despachosPorGuia.Values.Sum(d => d.CalcularPesoTotal()),
            CumplidosSLA = resumenGlobal.CumplidosSLA,
            IncumplidosSLA = resumenGlobal.IncumplidosSLA,
            PorcentajeCumplimientoSobreEntregados = resumenGlobal.PorcentajeCumplimiento
        };

        foreach (Establecimiento est in _establecimientos.Values)
        {
            dto.DesgloseComerciosOperativo.Add(ConstruirResumenOperativo(est.Codigo, est.NombreComercial, est.Despachos));

            string nombreTarifa = est.TarifaAcordada != null ? est.TarifaAcordada.TipoTarifa : "Sin Tarifa Asignada";
            decimal tarifaBase = est.TarifaAcordada != null ? est.TarifaAcordada.ValorBase : 0m;
            decimal totalComercio = est.CalcularTotalFacturable();

            dto.DesgloseComerciosFinanciero.Add(new ComercioFinancieroDto
            {
                CodigoComercio = est.Codigo,
                NombreComercio = est.NombreComercial,
                ModalidadTarifa = nombreTarifa,
                TarifaBase = tarifaBase,
                DespachosTotales = est.CantidadDespachos,
                DespachosFacturables = est.Despachos.Count(d => d.EsFacturable()),
                TotalFacturado = totalComercio
            });
            dto.GranTotalFacturado += totalComercio;
        }

        dto.CobrosPorModalidadTarifa = dto.DesgloseComerciosFinanciero
            .GroupBy(c => c.ModalidadTarifa)
            .Select(g => new CobroTarifaDto { ModalidadTarifa = g.Key, TotalFacturado = g.Sum(x => x.TotalFacturado) })
            .ToList();

        dto.DespachosEntregadosNoValorados = _despachosPorGuia.Values.Count(d =>
            d.EsFacturable() && (!_establecimientos.ContainsKey(d.CodigoEstablecimiento) || _establecimientos[d.CodigoEstablecimiento].TarifaAcordada == null));

        foreach (IGrouping<string, Despacho> grupo in _despachosPorGuia.Values.GroupBy(d => d.Modalidad != null ? d.Modalidad.NombreModalidad : "Sin Asignar"))
        {
            ComercioOperativoDto resumenMod = ConstruirResumenOperativo(grupo.Key, grupo.Key, grupo);
            dto.DesgloseModalidades.Add(new ModalidadOperativoDto
            {
                Modalidad = grupo.Key,
                TotalDespachos = resumenMod.TotalDespachos,
                Entregados = resumenMod.Entregados,
                CumplidosSLA = resumenMod.CumplidosSLA,
                IncumplidosSLA = resumenMod.IncumplidosSLA,
                PorcentajeCumplimiento = resumenMod.PorcentajeCumplimiento
            });
        }

        dto.TotalInconsistencias = _inconsistencias.Count;
        dto.ListadoInconsistencias = _inconsistencias.Select(InconsistenciaDto.MapToDto).ToList();
        dto.InconsistenciasPorTipo = _inconsistencias
            .GroupBy(i => i.Tipo ?? "Desconocido")
            .Select(g => new ConteoInconsistenciaDto { Clave = g.Key, Cantidad = g.Count() })
            .ToList();
        dto.InconsistenciasPorArchivo = _inconsistencias
            .GroupBy(i => i.ArchivoOrigen ?? "Desconocido")
            .Select(g => new ConteoInconsistenciaDto { Clave = g.Key, Cantidad = g.Count() })
            .ToList();

        return dto;
    }

    private ComercioOperativoDto ConstruirResumenOperativo(string codigo, string nombre, IEnumerable<Despacho> despachos)
    {
        List<Despacho> lista = despachos.ToList();
        int entregados = lista.Count(d => d.EstadoActual == Despacho.EstadoEntregado);
        int cumplidos = lista.Count(d => d.EstadoActual == Despacho.EstadoEntregado && d.EvaluarCumplimiento());

        return new ComercioOperativoDto
        {
            CodigoComercio = codigo,
            NombreComercio = nombre,
            TotalDespachos = lista.Count,
            Entregados = entregados,
            Devueltos = lista.Count(d => d.EstadoActual == Despacho.EstadoDevuelto),
            Pendientes = lista.Count(d => d.EstadoActual == Despacho.EstadoPendiente),
            EnDistribucion = lista.Count(d => d.EstadoActual == Despacho.EstadoEnDistribucion),
            CumplidosSLA = cumplidos,
            IncumplidosSLA = entregados - cumplidos,
            PorcentajeCumplimiento = entregados > 0 ? (cumplidos * 100.0) / entregados : 0.0
        };
    }

    public void MostrarInformeOperativo()
    {
        InformeConsolidadoDto op = MapToDto();
        Console.WriteLine($"Total despachos: {op.TotalDespachos}");
        Console.WriteLine($"Entregados: {op.TotalEntregados} | Devueltos: {op.TotalDevueltos} | En Distribucion: {op.TotalEnDistribucion} | Pendientes: {op.TotalPendientes}");
        Console.WriteLine($"Total paquetes: {op.TotalPaquetes} | Peso total: {op.PesoTotalKg:F2} kg");
        Console.WriteLine($"Cumplimiento SLA: {op.CumplidosSLA} cumplidos, {op.IncumplidosSLA} incumplidos");
        Console.WriteLine($"Porcentaje SLA sobre entregados: {op.PorcentajeCumplimientoSobreEntregados:F2}%");

        Console.WriteLine("\nDesglose por Comercio:");
        foreach (ComercioOperativoDto com in op.DesgloseComerciosOperativo)
        {
            Console.WriteLine($" - [{com.CodigoComercio}] {com.NombreComercio}: Total={com.TotalDespachos}, Entregados={com.Entregados}, Devueltos={com.Devueltos}, SLA={com.PorcentajeCumplimiento:F1}%");
        }

        Console.WriteLine("\nDesglose por Modalidad:");
        foreach (ModalidadOperativoDto mod in op.DesgloseModalidades)
        {
            Console.WriteLine($" - {mod.Modalidad}: Total={mod.TotalDespachos}, Entregados={mod.Entregados}, Cumplen={mod.CumplidosSLA}, SLA={mod.PorcentajeCumplimiento:F1}%");
        }
    }

    public void MostrarInformeFinanciero()
    {
        InformeConsolidadoDto fin = MapToDto();
        Console.WriteLine("Liquidacion por comercio:");
        foreach (ComercioFinancieroDto item in fin.DesgloseComerciosFinanciero)
        {
            Console.WriteLine($" - [{item.CodigoComercio}] {item.NombreComercio} ({item.ModalidadTarifa}): Facturables={item.DespachosFacturables}, Total=${item.TotalFacturado:N0}");
        }

        Console.WriteLine($"\nGran Total Facturado: ${fin.GranTotalFacturado:N0}");
        Console.WriteLine($"Despachos entregados no valorados: {fin.DespachosEntregadosNoValorados}");

        Despacho nx1001 = ObtenerDespacho("NX-1001");
        if (nx1001 != null && nx1001.EsFacturable())
        {
            Establecimiento est = ObtenerEstablecimiento("COM-01");
            if (est != null && est.TarifaAcordada != null)
            {
                decimal cobro = est.TarifaAcordada.CalcularTarifa(nx1001);
                Console.WriteLine($"\n[Prueba de control] Guia NX-1001: Peso={nx1001.CalcularPesoTotal():F1} kg | Cobro=${cobro:N0} (Esperado: $12.950)");
            }
        }
    }

    public void MostrarInformeInconsistencias()
    {
        InformeConsolidadoDto inc = MapToDto();
        Console.WriteLine($"Total inconsistencias registradas: {inc.TotalInconsistencias}");

        Console.WriteLine("\nPor tipo:");
        foreach (ConteoInconsistenciaDto par in inc.InconsistenciasPorTipo)
        {
            Console.WriteLine($" - {par.Clave}: {par.Cantidad}");
        }

        Console.WriteLine("\nPor archivo origen:");
        foreach (ConteoInconsistenciaDto par in inc.InconsistenciasPorArchivo)
        {
            Console.WriteLine($" - {par.Clave}: {par.Cantidad}");
        }

        Console.WriteLine("\nDetalle de inconsistencias:");
        int i = 1;
        foreach (InconsistenciaDto reg in inc.ListadoInconsistencias)
        {
            Console.WriteLine($" {i}. [{reg.Tipo}] ({reg.ArchivoOrigen} | {reg.IdentificadorReferencia}): {reg.Motivo}");
            i++;
        }
    }
}
