namespace NexoLog.Exportadores;

// Creador centralizado de instancias de exportadores de informes.
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
