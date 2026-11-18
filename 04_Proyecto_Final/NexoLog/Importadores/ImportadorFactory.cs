using NexoLog.DTOs;

namespace NexoLog.Importadores;

public static class ImportadorFactory
{
    public static IImportadorArchivo<AcuerdoDto> CrearAcuerdos(string formato)
    {
        return (formato ?? string.Empty).Trim().ToLower() switch
        {
            "json" => new ImportadorAcuerdosJson(),
            _ => throw new NexoLogException($"Formato no soportado para acuerdos: '{formato}'.")
        };
    }

    public static IImportadorArchivo<Despacho> CrearPedidos(string formato)
    {
        return (formato ?? string.Empty).Trim().ToLower() switch
        {
            "json" => new ImportadorPedidosJson(),
            "xml" => new ImportadorPedidosXml(),
            _ => throw new NexoLogException($"Formato no soportado para pedidos: '{formato}'.")
        };
    }

    public static IImportadorArchivo<Acontecimiento> CrearSeguimientos(string formato)
    {
        return (formato ?? string.Empty).Trim().ToLower() switch
        {
            "json" => new ImportadorSeguimientosJson(),
            "xml" => new ImportadorSeguimientosXml(),
            _ => throw new NexoLogException($"Formato no soportado para seguimientos: '{formato}'.")
        };
    }
}
