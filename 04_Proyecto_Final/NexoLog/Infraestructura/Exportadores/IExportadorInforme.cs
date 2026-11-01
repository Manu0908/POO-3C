using NexoLog.Servicios.Modelos;

namespace NexoLog.Infraestructura.Exportadores;

/// <summary>
/// Contrato para la exportación de los informes consolidados del sistema a archivos externos.
/// </summary>
public interface IExportadorInforme
{
    /// <summary>
    /// Exporta los tres informes consolidados al destino y formato correspondiente.
    /// </summary>
    /// <param name="operativo">Informe operativo consolidado.</param>
    /// <param name="financiero">Informe financiero de liquidación.</param>
    /// <param name="inconsistencias">Informe de auditoría de inconsistencias.</param>
    /// <param name="rutaDestino">Ruta de destino del archivo resultante.</param>
    void Exportar(
        InformeOperativo operativo,
        InformeFinanciero financiero,
        InformeInconsistencias inconsistencias,
        string rutaDestino);
}
