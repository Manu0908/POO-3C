using NexoLog.DTOs;

namespace NexoLog.Exportadores;

public interface IExportadorInforme
{
    string Extension { get; }
    void Exportar(InformeConsolidadoDto dto, string rutaDestino);
}
