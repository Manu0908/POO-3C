using NexoLog.Tarifas;

namespace NexoLog.Informes;

// Detalle de liquidacion financiera por comercio.
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

// Informe consolidado con resultados financieros y cobros por tarifa.
public class InformeFinanciero
{
    public List<ItemFinancieroComercio> DesgloseComercios { get; set; } = new List<ItemFinancieroComercio>();
    public Dictionary<string, decimal> CobrosPorModalidadTarifa { get; set; } = new Dictionary<string, decimal>();
    public decimal GranTotalFacturado { get; set; }
    public int DespachosEntregadosNoValorados { get; set; }

    public static InformeFinanciero Crear(IEnumerable<Establecimiento> establecimientos, IEnumerable<Despacho> despachos)
    {
        InformeFinanciero informe = new InformeFinanciero();
        decimal granTotal = 0m;
        int noValorados = 0;
        Dictionary<string, Establecimiento> mapaEstablecimientos = new Dictionary<string, Establecimiento>(StringComparer.OrdinalIgnoreCase);

        foreach (Establecimiento est in establecimientos)
        {
            mapaEstablecimientos[est.Codigo] = est;

            ItemFinancieroComercio item = new ItemFinancieroComercio();
            item.CodigoComercio = est.Codigo;
            item.NombreComercio = est.NombreComercial;

            string nombreTarifa = est.TarifaAcordada != null ? est.TarifaAcordada.ObtenerTipoTarifa() : "Sin Tarifa Asignada";
            item.ModalidadTarifa = nombreTarifa;

            decimal tarifaBase = est.TarifaAcordada != null ? est.TarifaAcordada.ValorBase : 0m;

            item.TarifaBase = tarifaBase;
            item.DespachosTotales = est.CantidadDespachos;

            int facturables = 0;
            foreach (Despacho d in est.Despachos)
            {
                if (d.EsFacturable())
                {
                    facturables++;
                }
            }

            item.DespachosFacturables = facturables;
            decimal totalComercio = est.CalcularTotalFacturable();
            item.TotalFacturado = totalComercio;
            granTotal += totalComercio;

            informe.DesgloseComercios.Add(item);

            if (!informe.CobrosPorModalidadTarifa.ContainsKey(nombreTarifa))
            {
                informe.CobrosPorModalidadTarifa[nombreTarifa] = 0m;
            }
            informe.CobrosPorModalidadTarifa[nombreTarifa] += totalComercio;
        }

        foreach (Despacho d in despachos)
        {
            if (d.EsFacturable())
            {
                Establecimiento est;
                mapaEstablecimientos.TryGetValue(d.CodigoEstablecimiento, out est);
                if (est == null || est.TarifaAcordada == null)
                {
                    noValorados++;
                }
            }
        }

        informe.GranTotalFacturado = granTotal;
        informe.DespachosEntregadosNoValorados = noValorados;
        return informe;
    }
}
