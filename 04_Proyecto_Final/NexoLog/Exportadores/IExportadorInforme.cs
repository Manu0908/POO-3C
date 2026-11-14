using NexoLog.Informes;

namespace NexoLog.Exportadores;

public interface IExportadorInforme
{
    string Extension { get; }
    void Exportar(InformeOperativo operativo, InformeFinanciero financiero, InformeInconsistencias inconsistencias, string rutaDestino);
}
