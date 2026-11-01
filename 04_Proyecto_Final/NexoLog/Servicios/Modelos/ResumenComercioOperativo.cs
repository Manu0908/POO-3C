namespace NexoLog.Servicios.Modelos;

/// <summary>
/// Resumen métrico operativo desglosado por comercio remitente.
/// </summary>
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
