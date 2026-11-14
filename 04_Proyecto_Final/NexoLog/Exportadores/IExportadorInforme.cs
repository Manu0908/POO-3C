using NexoLog.Informes;

namespace NexoLog.Exportadores;

public interface IExportadorInforme
{
    void Exportar(InformeOperativo operativo, InformeFinanciero financiero, InformeInconsistencias inconsistencias, string rutaDestino);
}
