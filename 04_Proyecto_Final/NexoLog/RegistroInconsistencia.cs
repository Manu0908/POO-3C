namespace NexoLog;

// Registra una inconsistencia o irregularidad detectada en los archivos procesados.
public class RegistroInconsistencia
{
    // Constantes
    public const string TipoDuplicado = "Duplicado";
    public const string TipoDatoInvalido = "DatoInvalido";
    public const string TipoReferenciaInexistente = "ReferenciaInexistente";
    public const string TipoAcontecimientoIncompatible = "AcontecimientoIncompatible";
    public const string TipoConflictoIdentificacion = "ConflictoIdentificacion";

    // Campos privados con validacion
    private string _motivo;

    // Constructor privado sin parametros para serializacion / reflexion
    private RegistroInconsistencia() { }

    // Constructor horizontal
    public RegistroInconsistencia(string archivoOrigen, string identificadorReferencia, string tipo, string motivo, DateTime fechaDeteccion)
    {
        ArchivoOrigen = archivoOrigen;
        IdentificadorReferencia = identificadorReferencia;
        Tipo = tipo;
        Motivo = motivo;
        FechaDeteccion = fechaDeteccion;
    }

    // Propiedades publicas
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

    // Metodos
    public string MostrarInformacion()
    {
        return $"[{FechaDeteccion:yyyy-MM-dd HH:mm:ss}] [{Tipo}] Archivo: '{ArchivoOrigen}' | Ref: '{IdentificadorReferencia}' | {Motivo}";
    }
}
