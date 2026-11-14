namespace NexoLog.Informes;

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

public class ResumenModalidadOperativo
{
    public string Modalidad { get; set; } = string.Empty;
    public int TotalDespachos { get; set; }
    public int Entregados { get; set; }
    public int CumplidosSLA { get; set; }
    public int IncumplidosSLA { get; set; }
    public double PorcentajeCumplimiento { get; set; }
}

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
        int totalPaquetes = 0;
        double pesoTotal = 0.0;

        foreach (Despacho d in listaDespachos)
        {
            totalPaquetes += d.CantidadPaquetes;
            pesoTotal += d.CalcularPesoTotal();
        }

        int entregados;
        int devueltos;
        int pendientes;
        int enDistribucion;
        int cumplidos;
        int incumplidos;

        ContarEstadosYCumplimiento(listaDespachos,
            out entregados, out devueltos, out pendientes, out enDistribucion,
            out cumplidos, out incumplidos);

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

            int entC;
            int devC;
            int penC;
            int distC;
            int cumpC;
            int incumpC;

            ContarEstadosYCumplimiento(est.Despachos,
                out entC, out devC, out penC, out distC,
                out cumpC, out incumpC);

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

            int entM;
            int devM;
            int penM;
            int distM;
            int cumpM;
            int incumpM;

            ContarEstadosYCumplimiento(grupo.Value,
                out entM, out devM, out penM, out distM,
                out cumpM, out incumpM);

            mod.Entregados = entM;
            mod.CumplidosSLA = cumpM;
            mod.IncumplidosSLA = incumpM;
            mod.PorcentajeCumplimiento = entM > 0 ? (cumpM * 100.0) / entM : 0.0;

            informe.DesgloseModalidades.Add(mod);
        }

        return informe;
    }

    private static void ContarEstadosYCumplimiento(
        IEnumerable<Despacho> despachos,
        out int entregados,
        out int devueltos,
        out int pendientes,
        out int enDistribucion,
        out int cumplidos,
        out int incumplidos)
    {
        entregados = 0;
        devueltos = 0;
        pendientes = 0;
        enDistribucion = 0;
        cumplidos = 0;
        incumplidos = 0;

        foreach (Despacho d in despachos)
        {
            switch (d.EstadoActual)
            {
                case Despacho.EstadoEntregado:
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
                case Despacho.EstadoDevuelto:
                    devueltos++;
                    break;
                case Despacho.EstadoEnDistribucion:
                    enDistribucion++;
                    break;
                default:
                    pendientes++;
                    break;
            }
        }
    }
}
