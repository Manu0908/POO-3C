namespace NexoLog.Dominio;

/// <summary>
/// Catálogo de tipos de acontecimientos o hitos en la trazabilidad de un despacho.
/// </summary>
public enum TipoAcontecimiento
{
    REC, // Mercancía recibida en centro logístico
    RUT, // Salida a distribución en ruta
    ENT, // Entrega exitosa al destinatario
    DEV  // Devolución al remitente
}
