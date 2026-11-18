using NexoLog.DTOs;

namespace NexoLog.Exportadores;

public interface IExportadorInforme
{
    string TipoArchivo { get; }
    void Exportar(InformeConsolidadoDto dto);
}
