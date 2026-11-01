namespace NexoLog.Dominio.Inconsistencias;

/// <summary>
/// Registra una inconsistencia o infracción detectada durante la validación de archivos e insumos operativos.
/// </summary>
public class RegistroInconsistencia
{
    private string _archivoOrigen;
    private string _identificadorReferencia;
    private TipoInconsistencia _tipo;
    private string _motivo;
    private DateTime _fechaDeteccion;

    public string ArchivoOrigen
    {
        get { return _archivoOrigen; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NexoLogException("El nombre del archivo origen es obligatorio.");
            }
            _archivoOrigen = value.Trim();
        }
    }

    public string IdentificadorReferencia
    {
        get { return _identificadorReferencia; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NexoLogException("El identificador de referencia de la inconsistencia es obligatorio.");
            }
            _identificadorReferencia = value.Trim();
        }
    }

    public TipoInconsistencia Tipo
    {
        get { return _tipo; }
        private set { _tipo = value; }
    }

    public string Motivo
    {
        get { return _motivo; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NexoLogException("El motivo o descripción de la inconsistencia es obligatorio.");
            }
            _motivo = value.Trim();
        }
    }

    public DateTime FechaDeteccion
    {
        get { return _fechaDeteccion; }
        private set { _fechaDeteccion = value; }
    }

    public RegistroInconsistencia(
        string archivoOrigen,
        string identificadorReferencia,
        TipoInconsistencia tipo,
        string motivo,
        DateTime fechaDeteccion)
    {
        ArchivoOrigen = archivoOrigen;
        IdentificadorReferencia = identificadorReferencia;
        Tipo = tipo;
        Motivo = motivo;
        FechaDeteccion = fechaDeteccion;
    }

    public override string ToString()
    {
        return $"[{FechaDeteccion:yyyy-MM-dd HH:mm:ss}] [{Tipo}] Archivo: '{ArchivoOrigen}' | Ref: '{IdentificadorReferencia}' | {Motivo}";
    }
}
