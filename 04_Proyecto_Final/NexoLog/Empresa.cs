using NexoLog.DTOs;

namespace NexoLog;

public class Empresa
{
    private List<Establecimiento> _establecimientos;
    private List<Despacho> _despachos;
    private List<RegistroInconsistencia> _inconsistencias;

    public List<Establecimiento> Establecimientos { get { return new List<Establecimiento>(_establecimientos); } }
    public List<Despacho> Despachos { get { return new List<Despacho>(_despachos); } }
    public List<RegistroInconsistencia> Inconsistencias { get { return new List<RegistroInconsistencia>(_inconsistencias); } }

    public Empresa()
    {
        _establecimientos = new List<Establecimiento>();
        _despachos = new List<Despacho>();
        _inconsistencias = new List<RegistroInconsistencia>();
    }

    public void Limpiar()
    {
        _establecimientos.Clear();
        _despachos.Clear();
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

        Establecimiento existente = ObtenerEstablecimiento(nuevoEstablecimiento.Codigo);
        if (existente != null)
        {
            if (nuevoEstablecimiento.TarifaAcordada != null)
            {
                existente.AsignarTarifa(nuevoEstablecimiento.TarifaAcordada);
            }
        }
        else
        {
            _establecimientos.Add(nuevoEstablecimiento);
        }
    }

    public void RegistrarDespacho(Despacho despacho, string archivoOrigen)
    {
        if (despacho == null)
        {
            return;
        }

        string guia = despacho.NumeroGuia;
        Despacho existente = ObtenerDespacho(guia);
        if (existente != null)
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
        Establecimiento est = ObtenerEstablecimiento(codEst);
        if (est == null)
        {
            est = new Establecimiento(codEst, $"Comercio {codEst}");
            _establecimientos.Add(est);
        }

        est.AgregarDespacho(despacho);
        _despachos.Add(despacho);
    }

    public void RegistrarSeguimientos(List<Acontecimiento> acontecimientos, string archivoOrigen)
    {
        List<string> guias = acontecimientos.Select(e => e.NumeroGuia.ToUpper()).Distinct().ToList();

        foreach (string guia in guias)
        {
            List<Acontecimiento> eventosGuia = acontecimientos.Where(e => e.NumeroGuia.ToUpper() == guia).ToList();
            Despacho despacho = ObtenerDespacho(guia);

            if (despacho == null)
            {
                foreach (Acontecimiento e in eventosGuia)
                {
                    _inconsistencias.Add(new RegistroInconsistencia(
                        archivoOrigen,
                        e.IdAcontecimiento,
                        RegistroInconsistencia.TipoReferenciaInexistente,
                        $"El acontecimiento '{e.IdAcontecimiento}' referencia a una guia inexistente: '{e.NumeroGuia}'.",
                        DateTime.Now));
                }
                continue;
            }

            RegistrarEventosEnDespacho(despacho, eventosGuia, archivoOrigen);
        }
    }

    private void RegistrarEventosEnDespacho(Despacho despacho, List<Acontecimiento> eventos, string archivoOrigen)
    {
        List<Acontecimiento> validos = new List<Acontecimiento>();

        foreach (Acontecimiento evt in eventos)
        {
            Acontecimiento prev = validos.FirstOrDefault(v => v.IdAcontecimiento.ToLower() == evt.IdAcontecimiento.ToLower());
            if (prev != null)
            {
                bool identico = prev.Tipo == evt.Tipo && prev.FechaHora == evt.FechaHora;
                string tipo = identico ? RegistroInconsistencia.TipoDuplicado : RegistroInconsistencia.TipoConflictoIdentificacion;
                string motivo = identico
                    ? $"Acontecimiento '{evt.IdAcontecimiento}' duplicado identicamente en la guia '{despacho.NumeroGuia}'."
                    : $"Conflicto en acontecimiento '{evt.IdAcontecimiento}' de la guia '{despacho.NumeroGuia}': mismo ID con datos contradictorios.";
                _inconsistencias.Add(new RegistroInconsistencia(archivoOrigen, evt.IdAcontecimiento, tipo, motivo, DateTime.Now));
                continue;
            }
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
        return _despachos.FirstOrDefault(d => d.NumeroGuia.ToLower() == guia.ToLower());
    }

    public Establecimiento ObtenerEstablecimiento(string codigo)
    {
        return _establecimientos.FirstOrDefault(e => e.Codigo.ToLower() == codigo.ToLower());
    }

    public InformeConsolidadoDto MapToDto()
    {
        ComercioOperativoDto resumenGlobal = ConstruirResumenOperativo("TOTAL", "Global", _despachos);
        InformeConsolidadoDto dto = new InformeConsolidadoDto
        {
            FechaGeneracion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            Sistema = "NexoLog S.A.S.",
            TotalDespachos = resumenGlobal.TotalDespachos,
            TotalEntregados = resumenGlobal.Entregados,
            TotalDevueltos = resumenGlobal.Devueltos,
            TotalPendientes = resumenGlobal.Pendientes,
            TotalEnDistribucion = resumenGlobal.EnDistribucion,
            TotalPaquetes = _despachos.Sum(d => d.CantidadPaquetes),
            PesoTotalKg = _despachos.Sum(d => d.CalcularPesoTotal()),
            CumplidosSLA = resumenGlobal.CumplidosSLA,
            IncumplidosSLA = resumenGlobal.IncumplidosSLA,
            PorcentajeCumplimientoSobreEntregados = resumenGlobal.PorcentajeCumplimiento
        };

        foreach (Establecimiento est in _establecimientos)
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

        List<string> tarifas = dto.DesgloseComerciosFinanciero.Select(c => c.ModalidadTarifa).Distinct().ToList();
        foreach (string tarifa in tarifas)
        {
            decimal totalTarifa = dto.DesgloseComerciosFinanciero
                .Where(c => c.ModalidadTarifa == tarifa)
                .Sum(c => c.TotalFacturado);
            dto.CobrosPorModalidadTarifa.Add(new CobroTarifaDto { ModalidadTarifa = tarifa, TotalFacturado = totalTarifa });
        }

        dto.DespachosEntregadosNoValorados = _despachos.Count(d => EstaEntregadoSinTarifa(d));

        List<string> modalidades = _despachos
            .Select(d => d.Modalidad != null ? d.Modalidad.NombreModalidad : "Sin Asignar")
            .Distinct()
            .ToList();
        foreach (string mod in modalidades)
        {
            List<Despacho> grupo = _despachos
                .Where(d => (d.Modalidad != null ? d.Modalidad.NombreModalidad : "Sin Asignar") == mod)
                .ToList();
            ComercioOperativoDto resumenMod = ConstruirResumenOperativo(mod, mod, grupo);
            dto.DesgloseModalidades.Add(new ModalidadOperativoDto
            {
                Modalidad = mod,
                TotalDespachos = resumenMod.TotalDespachos,
                Entregados = resumenMod.Entregados,
                CumplidosSLA = resumenMod.CumplidosSLA,
                IncumplidosSLA = resumenMod.IncumplidosSLA,
                PorcentajeCumplimiento = resumenMod.PorcentajeCumplimiento
            });
        }

        dto.TotalInconsistencias = _inconsistencias.Count;
        dto.ListadoInconsistencias = _inconsistencias.Select(InconsistenciaDto.MapToDto).ToList();

        List<string> tiposInc = _inconsistencias.Select(i => i.Tipo ?? "Desconocido").Distinct().ToList();
        foreach (string t in tiposInc)
        {
            int cantidad = _inconsistencias.Count(i => (i.Tipo ?? "Desconocido") == t);
            dto.InconsistenciasPorTipo.Add(new ConteoInconsistenciaDto { Clave = t, Cantidad = cantidad });
        }

        List<string> archivosInc = _inconsistencias.Select(i => i.ArchivoOrigen ?? "Desconocido").Distinct().ToList();
        foreach (string arch in archivosInc)
        {
            int cantidad = _inconsistencias.Count(i => (i.ArchivoOrigen ?? "Desconocido") == arch);
            dto.InconsistenciasPorArchivo.Add(new ConteoInconsistenciaDto { Clave = arch, Cantidad = cantidad });
        }

        return dto;
    }

    private bool EstaEntregadoSinTarifa(Despacho despacho)
    {
        if (!despacho.EsFacturable())
        {
            return false;
        }
        Establecimiento est = ObtenerEstablecimiento(despacho.CodigoEstablecimiento);
        return est == null || est.TarifaAcordada == null;
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
