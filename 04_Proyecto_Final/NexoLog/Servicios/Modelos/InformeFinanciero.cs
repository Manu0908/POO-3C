namespace NexoLog.Servicios.Modelos;

/// <summary>
/// Modelo estructurado para el Informe Financiero consolidado de liquidación en NexoLog.
/// </summary>
public class InformeFinanciero
{
    public List<ItemFinancieroComercio> DesgloseComercios { get; set; } = new List<ItemFinancieroComercio>();
    public Dictionary<string, decimal> CobrosPorModalidadTarifa { get; set; } = new Dictionary<string, decimal>();
    public decimal GranTotalFacturado { get; set; }
    public int DespachosEntregadosNoValorados { get; set; }
}
