namespace NexoLog.Servicios.Modelos;

/// <summary>
/// Resumen métrico operativo agrupado por modalidad de servicio (SLA).
/// </summary>
public class ResumenModalidadOperativo
{
    public string Modalidad { get; set; } = string.Empty;
    public int TotalDespachos { get; set; }
    public int Entregados { get; set; }
    public int CumplidosSLA { get; set; }
    public int IncumplidosSLA { get; set; }
    public double PorcentajeCumplimiento { get; set; }
}
