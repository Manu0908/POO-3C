using NexoLog.Dominio.Inconsistencias;

namespace NexoLog.Infraestructura.Lectores;

/// <summary>
/// Contrato genérico para los lectores de archivos de datos del sistema NexoLog.
/// </summary>
/// <typeparam name="T">Tipo de dato de dominio o DTO retornado tras la lectura.</typeparam>
public interface ILectorArchivo<T>
{
    /// <summary>
    /// Lee e interpreta el contenido del archivo especificado, capturando inconsistencias en el camino sin abortar la operación.
    /// </summary>
    /// <param name="rutaArchivo">Ruta absoluta o relativa del archivo a procesar.</param>
    /// <param name="inconsistencias">Lista receptora donde se registrarán anomalías de datos.</param>
    /// <returns>Lista de entidades válidas obtenidas.</returns>
    List<T> Leer(string rutaArchivo, List<RegistroInconsistencia> inconsistencias);
}
