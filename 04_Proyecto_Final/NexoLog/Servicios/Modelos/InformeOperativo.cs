namespace NexoLog.Servicios.Modelos;

/// <summary>
/// Modelo estructurado para el Informe Operativo de la operación de NexoLog.
/// </summary>
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
