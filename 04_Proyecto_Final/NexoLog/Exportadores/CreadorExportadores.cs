namespace NexoLog.Exportadores;

public static class CreadorExportadores
{
    public static IExportadorInforme CrearExportadorJson()
    {
        return new ExportadorInformesJson();
    }

    public static IExportadorInforme CrearExportadorXml()
    {
        return new ExportadorInformesXml();
    }
}
