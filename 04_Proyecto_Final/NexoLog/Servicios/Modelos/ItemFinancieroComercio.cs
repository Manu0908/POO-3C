namespace NexoLog.Servicios.Modelos;

/// <summary>
/// Desglose de liquidación y facturación por comercio remitente.
/// </summary>
public class ItemFinancieroComercio
{
    public string CodigoComercio { get; set; } = string.Empty;
    public string NombreComercio { get; set; } = string.Empty;
    public string ModalidadTarifa { get; set; } = string.Empty;
    public decimal TarifaBase { get; set; }
    public int DespachosTotales { get; set; }
    public int DespachosFacturables { get; set; }
    public decimal TotalFacturado { get; set; }
}
