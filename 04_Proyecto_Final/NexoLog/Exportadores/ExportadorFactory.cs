namespace NexoLog.Exportadores;

public static class ExportadorFactory
{
    public static IExportadorInforme Crear(string formato)
    {
        return (formato ?? string.Empty).Trim().ToLower() switch
        {
            "1" => new ExportadorInformesJson(),
            "json" => new ExportadorInformesJson(),
            "2" => new ExportadorInformesXml(),
            "xml" => new ExportadorInformesXml(),
            _ => throw new NexoLogException($"Formato de exportacion no soportado: '{formato}'.")
        };
    }
}
