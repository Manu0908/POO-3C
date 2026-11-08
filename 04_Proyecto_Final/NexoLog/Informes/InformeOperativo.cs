namespace NexoLog.Informes;

// Resumen operativo agrupado por comercio remitente.
public class ResumenComercioOperativo
{
    public string CodigoComercio { get; set; } = string.Empty;
    public string NombreComercio { get; set; } = string.Empty;
    public int TotalDespachos { get; set; }
    public int Entregados { get; set; }
    public int Devueltos { get; set; }
    public int Pendientes { get; set; }
    public int EnDistribucion { get; set; }
    public int CumplidosSLA { get; set; }
    public int IncumplidosSLA { get; set; }
    public double PorcentajeCumplimiento { get; set; }
}

// Resumen operativo agrupado por modalidad de servicio.
public class ResumenModalidadOperativo
{
    public string Modalidad { get; set; } = string.Empty;
    public int TotalDespachos { get; set; }
    public int Entregados { get; set; }
    public int CumplidosSLA { get; set; }
    public int IncumplidosSLA { get; set; }
    public double PorcentajeCumplimiento { get; set; }
}

// Informe consolidado con metricas operativas y niveles de servicio SLA.
public class InformeOperativo
{
    public int TotalDespachos { get; set; }
    public int TotalEntregados { get; set; }
    public int TotalDevueltos { get; set; }
    public int TotalPendientes { get; set; }
    public int TotalEnDistribucion { get; set; }
    public int TotalPaquetes { get; set; }
    public double PesoTotalKg { get; set; }
    public int CumplidosSLA { get; set; }
    public int IncumplidosSLA { get; set; }
    public double PorcentajeCumplimientoSobreEntregados { get; set; }
    public List<ResumenComercioOperativo> DesgloseComercios { get; set; } = new List<ResumenComercioOperativo>();
    public List<ResumenModalidadOperativo> DesgloseModalidades { get; set; } = new List<ResumenModalidadOperativo>();

    public static InformeOperativo Crear(IEnumerable<Despacho> despachos, IEnumerable<Establecimiento> establecimientos)
    {
        InformeOperativo informe = new InformeOperativo();
        List<Despacho> listaDespachos = despachos.ToList();

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
        informe.PorcentajeCumplimientoSobreEntregados = entregados > 0 ? (cumplidos * 100.0) / entregados : 0.0;

        foreach (Establecimiento est in establecimientos)
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
                    if (d.EvaluarCumplimiento())
                    {
                        cumpC++;
                    }
                    else
                    {
                        incumpC++;
                    }
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

        Dictionary<string, List<Despacho>> porModalidad = new Dictionary<string, List<Despacho>>(StringComparer.OrdinalIgnoreCase);
        foreach (Despacho d in listaDespachos)
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
                    if (d.EvaluarCumplimiento())
                    {
                        cumpM++;
                    }
                    else
                    {
                        incumpM++;
                    }
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
}
