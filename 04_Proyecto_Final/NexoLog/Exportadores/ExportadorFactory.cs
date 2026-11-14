namespace NexoLog.Exportadores;

public static class ExportadorFactory
{
    public static IExportadorInforme Crear(string formato)
    {
        string formatoNormalizado = (formato ?? string.Empty).Trim().ToLowerInvariant();
        switch (formatoNormalizado)
        {
            case "1":
            case "json":
                return new ExportadorInformesJson();
            case "2":
            case "xml":
                return new ExportadorInformesXml();
            default:
                throw new NexoLogException($"Formato de exportacion no soportado: '{formato}'.");
        }
    }
}
