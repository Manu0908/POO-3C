namespace NexoLog.Lectores;

// Contrato generico para lectores de archivos de datos del sistema.
public interface ILectorArchivo<T>
{
    List<T> Leer(string rutaArchivo, List<RegistroInconsistencia> inconsistencias);
}
