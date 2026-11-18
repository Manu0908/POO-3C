namespace NexoLog.Exportadores;

public static class ExportadorFactory
{
    public static IExportadorInforme Crear(string formato)
    {
        if (string.IsNullOrWhiteSpace(formato))
        {
            throw new NexoLogException("Debe indicar un formato de exportacion (json o xml).");
        }

        var tipo = formato.Trim().ToLower();
        return tipo switch
        {
            "json" => new ExportadorInformesJson(),
            "xml" => new ExportadorInformesXml(),
            _ => throw new NexoLogException($"Formato de exportacion no soportado: '{formato}'.")
        };
    }
}
