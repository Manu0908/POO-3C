namespace NexoLog.Dominio.Inconsistencias;

/// <summary>
/// Tipos de anomalías e inconsistencias detectables durante la lectura y procesamiento de lotes logísticos.
/// </summary>
public enum TipoInconsistencia
{
    Duplicado,
    DatoInvalido,
    ReferenciaInexistente,
    AcontecimientoIncompatible,
    ConflictoIdentificacion
}
