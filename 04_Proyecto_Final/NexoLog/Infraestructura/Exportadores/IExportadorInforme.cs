using NexoLog.Servicios.Modelos;

namespace NexoLog.Infraestructura.Exportadores;

// Contrato para la exportación de los informes consolidados del sistema.
public interface IExportadorInforme
{
    void Exportar(
        InformeOperativo operativo,
        InformeFinanciero financiero,
        InformeInconsistencias inconsistencias,
        string rutaDestino);
}
