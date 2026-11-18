using NexoLog.DTOs;

namespace NexoLog;

public class RegistroInconsistencia
{
    public const string TipoDuplicado = "Duplicado";
    public const string TipoDatoInvalido = "DatoInvalido";
    public const string TipoReferenciaInexistente = "ReferenciaInexistente";
    public const string TipoAcontecimientoIncompatible = "AcontecimientoIncompatible";
    public const string TipoConflictoIdentificacion = "ConflictoIdentificacion";

    private string _motivo;

    private RegistroInconsistencia() { }

    public RegistroInconsistencia(string archivoOrigen, string identificadorReferencia, string tipo, string motivo, DateTime fechaDeteccion)
    {
        ArchivoOrigen = archivoOrigen;
        IdentificadorReferencia = identificadorReferencia;
        Tipo = tipo;
        Motivo = motivo;
        FechaDeteccion = fechaDeteccion;
    }

    public string ArchivoOrigen { get; private set; }
    public string IdentificadorReferencia { get; private set; }
    public string Tipo { get; private set; }
    public DateTime FechaDeteccion { get; private set; }

    public string Motivo
    {
        get { return _motivo; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NexoLogException("El motivo o descripcion de la inconsistencia es obligatorio.");
            }
            _motivo = value.Trim();
        }
    }

    public InconsistenciaDto MapToDto()
    {
        return new InconsistenciaDto
        {
            ArchivoOrigen = ArchivoOrigen,
            IdentificadorReferencia = IdentificadorReferencia,
            Tipo = Tipo,
            Motivo = Motivo,
            FechaDeteccion = FechaDeteccion
        };
    }

    public string MostrarInformacion()
    {
        return $"[{FechaDeteccion}] [{Tipo}] Archivo: '{ArchivoOrigen}' | Ref: '{IdentificadorReferencia}' | {Motivo}";
    }
}
