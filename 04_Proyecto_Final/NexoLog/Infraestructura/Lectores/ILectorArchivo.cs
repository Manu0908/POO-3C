using NexoLog.Dominio.Inconsistencias;

namespace NexoLog.Infraestructura.Lectores;

// Contrato genérico para los lectores de archivos de datos del sistema.
public interface ILectorArchivo<T>
{
    List<T> Leer(string rutaArchivo, List<RegistroInconsistencia> inconsistencias);
}
