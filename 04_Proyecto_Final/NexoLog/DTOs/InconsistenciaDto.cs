namespace NexoLog.DTOs;

public class InconsistenciaDto
{
    public string ArchivoOrigen { get; set; }
    public string IdentificadorReferencia { get; set; }
    public string Tipo { get; set; }
    public string Motivo { get; set; }
    public DateTime FechaDeteccion { get; set; }
}
