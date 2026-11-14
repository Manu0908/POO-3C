using NexoLog.DTOs;

namespace NexoLog.Importadores;

public static class ImportadorFactory
{
    public static IImportadorArchivo<AcuerdoDto> CrearAcuerdos(string formato)
    {
        string formatoNormalizado = (formato ?? string.Empty).Trim().ToLowerInvariant();
        switch (formatoNormalizado)
        {
            case "json":
                return new ImportadorAcuerdosJson();
            default:
                throw new NexoLogException($"Formato no soportado para acuerdos: '{formato}'.");
        }
    }

    public static IImportadorArchivo<Despacho> CrearPedidos(string formato)
    {
        string formatoNormalizado = (formato ?? string.Empty).Trim().ToLowerInvariant();
        switch (formatoNormalizado)
        {
            case "json":
                return new ImportadorPedidosJson();
            case "xml":
                return new ImportadorPedidosXml();
            default:
                throw new NexoLogException($"Formato no soportado para pedidos: '{formato}'.");
        }
    }

    public static IImportadorArchivo<Acontecimiento> CrearSeguimientos(string formato)
    {
        string formatoNormalizado = (formato ?? string.Empty).Trim().ToLowerInvariant();
        switch (formatoNormalizado)
        {
            case "json":
                return new ImportadorSeguimientosJson();
            case "xml":
                return new ImportadorSeguimientosXml();
            default:
                throw new NexoLogException($"Formato no soportado para seguimientos: '{formato}'.");
        }
    }
}
