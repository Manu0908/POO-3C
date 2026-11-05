using NexoLog.Informes;

namespace NexoLog.Exportadores;

// Contrato para la exportacion de los informes consolidados del sistema.
public interface IExportadorInforme
{
    void Exportar(InformeOperativo operativo, InformeFinanciero financiero, InformeInconsistencias inconsistencias, string rutaDestino);
}
