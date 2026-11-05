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
}
