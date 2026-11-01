using NexoLog.Dominio;
using NexoLog.Dominio.Inconsistencias;
using NexoLog.Dominio.Tarifas;
using NexoLog.Servicios.Modelos;

namespace NexoLog.Servicios;

/// <summary>
/// Servicio encargado de compilar y generar los informes operativos, financieros y de inconsistencias.
/// </summary>
public class ServicioInformes
{
    /// <summary>
    /// Genera el consolidado operativo a nivel global, por comercio remitente y por modalidad de servicio.
    /// </summary>
    public InformeOperativo GenerarInformeOperativo(
        IEnumerable<Despacho> despachos,
        IEnumerable<Establecimiento> establecimientos)
    {
        InformeOperativo informe = new InformeOperativo();
        List<Despacho> listaDespachos = despachos != null ? despachos.ToList() : new List<Despacho>();
        List<Establecimiento> listaEstablecimientos = establecimientos != null ? establecimientos.ToList() : new List<Establecimiento>();

        informe.TotalDespachos = listaDespachos.Count;

        int entregados = 0;
        int devueltos = 0;
        int pendientes = 0;
        int enDistribucion = 0;
        int totalPaquetes = 0;
        double pesoTotal = 0.0;
        int cumplidos = 0;
        int incumplidos = 0;

        foreach (Despacho d in listaDespachos)
        {
            totalPaquetes += d.CantidadPaquetes;
            pesoTotal += d.CalcularPesoTotal();

            switch (d.EstadoActual)
            {
                case EstadoDespacho.Entregado:
                    entregados++;
                    if (d.EvaluarCumplimiento())
                    {
                        cumplidos++;
                    }
                    else
                    {
                        incumplidos++;
                    }
                    break;

                case EstadoDespacho.Devuelto:
                    devueltos++;
                    break;

                case EstadoDespacho.EnDistribucion:
                    enDistribucion++;
                    break;

                case EstadoDespacho.Pendiente:
                default:
                    pendientes++;
                    break;
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
        informe.PorcentajeCumplimientoSobreEntregados = entregados > 0 ? (cumplidos * 100.0) / entregados : 0.0;

        // Desglose por comercio remitente
        foreach (Establecimiento est in listaEstablecimientos)
        {
            ResumenComercioOperativo resumenComercio = new ResumenComercioOperativo();
            resumenComercio.CodigoComercio = est.Codigo;
            resumenComercio.NombreComercio = est.NombreComercial;
            resumenComercio.TotalDespachos = est.CantidadDespachos;

            int entComercio = 0;
            int devComercio = 0;
            int penComercio = 0;
            int distComercio = 0;
            int cumpComercio = 0;
            int incumpComercio = 0;

            foreach (Despacho d in est.Despachos)
            {
                if (d.EstadoActual == EstadoDespacho.Entregado)
                {
                    entComercio++;
                    if (d.EvaluarCumplimiento())
                    {
                        cumpComercio++;
                    }
                    else
                    {
                        incumpComercio++;
                    }
                }
                else if (d.EstadoActual == EstadoDespacho.Devuelto)
                {
                    devComercio++;
                }
                else if (d.EstadoActual == EstadoDespacho.EnDistribucion)
                {
                    distComercio++;
                }
                else
                {
                    penComercio++;
                }
            }

            resumenComercio.Entregados = entComercio;
            resumenComercio.Devueltos = devComercio;
            resumenComercio.Pendientes = penComercio;
            resumenComercio.EnDistribucion = distComercio;
            resumenComercio.CumplidosSLA = cumpComercio;
            resumenComercio.IncumplidosSLA = incumpComercio;
            resumenComercio.PorcentajeCumplimiento = entComercio > 0 ? (cumpComercio * 100.0) / entComercio : 0.0;

            informe.DesgloseComercios.Add(resumenComercio);
        }

        // Desglose por modalidad de servicio
        Dictionary<string, List<Despacho>> porModalidad = new Dictionary<string, List<Despacho>>(StringComparer.OrdinalIgnoreCase);

        foreach (Despacho d in listaDespachos)
        {
            string nombreModalidad = d.Modalidad != null ? d.Modalidad.NombreModalidad : "Sin Asignar";
            if (!porModalidad.ContainsKey(nombreModalidad))
            {
                porModalidad[nombreModalidad] = new List<Despacho>();
            }
            porModalidad[nombreModalidad].Add(d);
        }

        foreach (KeyValuePair<string, List<Despacho>> grupo in porModalidad)
        {
            ResumenModalidadOperativo resumenMod = new ResumenModalidadOperativo();
            resumenMod.Modalidad = grupo.Key;
            resumenMod.TotalDespachos = grupo.Value.Count;

            int entMod = 0;
            int cumpMod = 0;
            int incumpMod = 0;

            foreach (Despacho d in grupo.Value)
            {
                if (d.EstadoActual == EstadoDespacho.Entregado)
                {
                    entMod++;
                    if (d.EvaluarCumplimiento())
                    {
                        cumpMod++;
                    }
                    else
                    {
                        incumpMod++;
                    }
                }
            }

            resumenMod.Entregados = entMod;
            resumenMod.CumplidosSLA = cumpMod;
            resumenMod.IncumplidosSLA = incumpMod;
            resumenMod.PorcentajeCumplimiento = entMod > 0 ? (cumpMod * 100.0) / entMod : 0.0;

            informe.DesgloseModalidades.Add(resumenMod);
        }

        return informe;
    }

    /// <summary>
    /// Genera la liquidación financiera de despachos facturables por comercio y modalidad tarifaria.
    /// </summary>
    public InformeFinanciero GenerarInformeFinanciero(
        IEnumerable<Establecimiento> establecimientos,
        IEnumerable<Despacho> despachos)
    {
        InformeFinanciero informe = new InformeFinanciero();
        List<Establecimiento> listaEstablecimientos = establecimientos != null ? establecimientos.ToList() : new List<Establecimiento>();
        List<Despacho> listaDespachos = despachos != null ? despachos.ToList() : new List<Despacho>();

        decimal granTotal = 0m;
        int noValorados = 0;

        foreach (Establecimiento est in listaEstablecimientos)
        {
            ItemFinancieroComercio item = new ItemFinancieroComercio();
            item.CodigoComercio = est.Codigo;
            item.NombreComercio = est.NombreComercial;

            string nombreTarifa = est.TarifaAcordada != null ? est.TarifaAcordada.ObtenerTipoTarifa() : "Sin Tarifa Asignada";
            item.ModalidadTarifa = nombreTarifa;

            decimal tarifaBase = 0m;
            if (est.TarifaAcordada is TarifaPesoStrategy tPeso)
            {
                tarifaBase = tPeso.ValorPorKilogramo;
            }
            else if (est.TarifaAcordada is TarifaDistanciaStrategy tDist)
            {
                tarifaBase = tDist.ValorPorKilometro;
            }
            else if (est.TarifaAcordada is TarifaFijaStrategy tFija)
            {
                tarifaBase = tFija.TarifaFija;
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

            // Acumular cobro por modalidad de tarifa
            if (!informe.CobrosPorModalidadTarifa.ContainsKey(nombreTarifa))
            {
                informe.CobrosPorModalidadTarifa[nombreTarifa] = 0m;
            }
            informe.CobrosPorModalidadTarifa[nombreTarifa] += totalComercio;
        }

        // Revisar si hubo despachos entregados pero cuyo comercio carecía de tarifa
        foreach (Despacho d in listaDespachos)
        {
            if (d.EsFacturable())
            {
                Establecimiento est = listaEstablecimientos.FirstOrDefault(e => string.Equals(e.Codigo, d.CodigoEstablecimiento, StringComparison.OrdinalIgnoreCase));
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

    /// <summary>
    /// Compila y categoriza las inconsistencias operativas detectadas por tipo y por archivo fuente.
    /// </summary>
    public InformeInconsistencias GenerarInformeInconsistencias(IEnumerable<RegistroInconsistencia> inconsistencias)
    {
        InformeInconsistencias informe = new InformeInconsistencias();
        List<RegistroInconsistencia> lista = inconsistencias != null ? inconsistencias.ToList() : new List<RegistroInconsistencia>();

        informe.TotalInconsistencias = lista.Count;
        informe.Listado = lista;

        foreach (RegistroInconsistencia inc in lista)
        {
            // Agrupar por tipo
            if (!informe.InconsistenciasPorTipo.ContainsKey(inc.Tipo))
            {
                informe.InconsistenciasPorTipo[inc.Tipo] = 0;
            }
            informe.InconsistenciasPorTipo[inc.Tipo]++;

            // Agrupar por archivo origen
            string archivo = inc.ArchivoOrigen ?? "Desconocido";
            if (!informe.InconsistenciasPorArchivo.ContainsKey(archivo))
            {
                informe.InconsistenciasPorArchivo[archivo] = 0;
            }
            informe.InconsistenciasPorArchivo[archivo]++;
        }

        return informe;
    }
}
