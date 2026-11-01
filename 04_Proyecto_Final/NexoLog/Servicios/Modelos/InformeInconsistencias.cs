using NexoLog.Dominio.Inconsistencias;

namespace NexoLog.Servicios.Modelos;

/// <summary>
/// Modelo estructurado para el Informe de Auditoría e Inconsistencias de la integración.
/// </summary>
public class InformeInconsistencias
{
    public int TotalInconsistencias { get; set; }
    public Dictionary<TipoInconsistencia, int> InconsistenciasPorTipo { get; set; } = new Dictionary<TipoInconsistencia, int>();
    public Dictionary<string, int> InconsistenciasPorArchivo { get; set; } = new Dictionary<string, int>();
    public List<RegistroInconsistencia> Listado { get; set; } = new List<RegistroInconsistencia>();
}
