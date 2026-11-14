using NexoLog.DTOs;

namespace NexoLog.Informes;

// Informe consolidado de inconsistencias e infracciones detectadas.
public class InformeInconsistencias
{
    public int TotalInconsistencias { get; set; }
    public Dictionary<string, int> InconsistenciasPorTipo { get; set; } = new Dictionary<string, int>();
    public Dictionary<string, int> InconsistenciasPorArchivo { get; set; } = new Dictionary<string, int>();
    public List<InconsistenciaDto> Listado { get; set; } = new List<InconsistenciaDto>();

    public static InformeInconsistencias Crear(IEnumerable<RegistroInconsistencia> inconsistencias)
    {
        InformeInconsistencias informe = new InformeInconsistencias();
        List<RegistroInconsistencia> lista = inconsistencias.ToList();

        informe.TotalInconsistencias = lista.Count;

        foreach (RegistroInconsistencia inc in lista)
        {
            informe.Listado.Add(InconsistenciaDto.MapToDto(inc));

            string tipo = inc.Tipo ?? "Desconocido";
            if (!informe.InconsistenciasPorTipo.ContainsKey(tipo))
            {
                informe.InconsistenciasPorTipo[tipo] = 0;
            }
            informe.InconsistenciasPorTipo[tipo]++;

            string archivo = inc.ArchivoOrigen ?? "Desconocido";
            if (!informe.InconsistenciasPorArchivo.ContainsKey(archivo))
            {
                informe.InconsistenciasPorArchivo[archivo] = 0;
            }
            informe.InconsistenciasPorArchivo[archivo]++;
        }

        return informe;
    }
}
