namespace NexoLog.Importadores;

public interface IImportadorArchivo<T>
{
    List<T> Importar(string rutaArchivo, List<RegistroInconsistencia> inconsistencias);
}
