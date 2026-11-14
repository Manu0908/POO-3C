namespace NexoLog.DTOs;

// DTO de salida para transportar y serializar registros de inconsistencia sin exponer el dominio.
public class InconsistenciaDto
{
    public string ArchivoOrigen { get; set; } = string.Empty;
    public string IdentificadorReferencia { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;
    public DateTime FechaDeteccion { get; set; }

    public static InconsistenciaDto MapToDto(RegistroInconsistencia registro)
    {
        return new InconsistenciaDto
        {
            ArchivoOrigen = registro.ArchivoOrigen,
            IdentificadorReferencia = registro.IdentificadorReferencia,
            Tipo = registro.Tipo,
            Motivo = registro.Motivo,
            FechaDeteccion = registro.FechaDeteccion
        };
    }
}
