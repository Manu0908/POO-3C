using System.Text.Json.Serialization;

namespace NexoLog.DTOs;

public class ComercioOperativoDto
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

public class ModalidadOperativoDto
{
    public string Modalidad { get; set; } = string.Empty;
    public int TotalDespachos { get; set; }
    public int Entregados { get; set; }
    public int CumplidosSLA { get; set; }
    public int IncumplidosSLA { get; set; }
    public double PorcentajeCumplimiento { get; set; }
}

public class ComercioFinancieroDto
{
    public string CodigoComercio { get; set; } = string.Empty;
    public string NombreComercio { get; set; } = string.Empty;
    public string ModalidadTarifa { get; set; } = string.Empty;
    public decimal TarifaBase { get; set; }
    public int DespachosTotales { get; set; }
    public int DespachosFacturables { get; set; }
    public decimal TotalFacturado { get; set; }
}

public class CobroTarifaDto
{
    public string ModalidadTarifa { get; set; } = string.Empty;
    public decimal TotalFacturado { get; set; }
}

public class ConteoInconsistenciaDto
{
    public string Clave { get; set; } = string.Empty;
    public int Cantidad { get; set; }
}

public class InformeConsolidadoDto
{
    [JsonPropertyName("fechaGeneracion")]
    public string FechaGeneracion { get; set; } = string.Empty;

    [JsonPropertyName("sistema")]
    public string Sistema { get; set; } = string.Empty;

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
    public decimal GranTotalFacturado { get; set; }
    public int DespachosEntregadosNoValorados { get; set; }
    public int TotalInconsistencias { get; set; }

    public List<ComercioOperativoDto> DesgloseComerciosOperativo { get; set; } = new List<ComercioOperativoDto>();
    public List<ModalidadOperativoDto> DesgloseModalidades { get; set; } = new List<ModalidadOperativoDto>();
    public List<ComercioFinancieroDto> DesgloseComerciosFinanciero { get; set; } = new List<ComercioFinancieroDto>();
    public List<CobroTarifaDto> CobrosPorModalidadTarifa { get; set; } = new List<CobroTarifaDto>();
    public List<ConteoInconsistenciaDto> InconsistenciasPorTipo { get; set; } = new List<ConteoInconsistenciaDto>();
    public List<ConteoInconsistenciaDto> InconsistenciasPorArchivo { get; set; } = new List<ConteoInconsistenciaDto>();
    public List<InconsistenciaDto> ListadoInconsistencias { get; set; } = new List<InconsistenciaDto>();
}
