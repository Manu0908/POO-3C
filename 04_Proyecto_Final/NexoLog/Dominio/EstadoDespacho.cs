namespace NexoLog.Dominio;

/// <summary>
/// Estados por los que transita un despacho dentro del flujo logístico.
/// </summary>
public enum EstadoDespacho
{
    Pendiente,
    EnDistribucion,
    Entregado,
    Devuelto
}
