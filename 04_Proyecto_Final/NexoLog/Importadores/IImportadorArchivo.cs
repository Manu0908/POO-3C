namespace NexoLog.Importadores;

// Contrato generico para importadores de archivos de datos del sistema.
public interface IImportadorArchivo<T>
{
    List<T> Importar(string rutaArchivo, List<RegistroInconsistencia> inconsistencias);
}
