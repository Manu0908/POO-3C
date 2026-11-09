using NexoLog.DTOs;

namespace NexoLog.Importadores;

// Creador centralizado de instancias de importadores de archivo.
public static class CreadorImportadores
{
    public static IImportadorArchivo<AcuerdoDto> CrearImportadorAcuerdosJson()
    {
        return new ImportadorAcuerdosJson();
    }

    public static IImportadorArchivo<Despacho> CrearImportadorPedidosJson()
    {
        return new ImportadorPedidosJson();
    }

    public static IImportadorArchivo<Despacho> CrearImportadorPedidosXml()
    {
        return new ImportadorPedidosXml();
    }

    public static IImportadorArchivo<Acontecimiento> CrearImportadorSeguimientosJson()
    {
        return new ImportadorSeguimientosJson();
    }

    public static IImportadorArchivo<Acontecimiento> CrearImportadorSeguimientosXml()
    {
        return new ImportadorSeguimientosXml();
    }
}
