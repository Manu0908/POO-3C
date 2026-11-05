namespace NexoLog.Informes;

// Informe consolidado de inconsistencias e infracciones detectadas.
public class InformeInconsistencias
{
    public int TotalInconsistencias { get; set; }
    public Dictionary<string, int> InconsistenciasPorTipo { get; set; } = new Dictionary<string, int>();
    public Dictionary<string, int> InconsistenciasPorArchivo { get; set; } = new Dictionary<string, int>();
    public List<RegistroInconsistencia> Listado { get; set; } = new List<RegistroInconsistencia>();
}
