namespace NexoLog.Servicios.Modelos;

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

public class InformeFinanciero
{
    public List<ItemFinancieroComercio> DesgloseComercios { get; set; } = new List<ItemFinancieroComercio>();
    public Dictionary<string, decimal> CobrosPorModalidadTarifa { get; set; } = new Dictionary<string, decimal>();
    public decimal GranTotalFacturado { get; set; }
    public int DespachosEntregadosNoValorados { get; set; }
}
