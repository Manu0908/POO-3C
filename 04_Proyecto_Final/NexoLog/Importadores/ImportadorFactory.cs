using NexoLog.DTOs;

namespace NexoLog.Importadores;

public static class ImportadorFactory
{
    public static IImportadorArchivo<AcuerdoDto> CrearAcuerdos(string formato)
    {
        if (string.IsNullOrWhiteSpace(formato))
        {
            throw new NexoLogException("Debe indicar un formato para acuerdos.");
        }

        var tipo = formato.Trim().ToLower();
        return tipo switch
        {
            "json" => new ImportadorAcuerdosJson(),
            _ => throw new NexoLogException($"Formato no soportado para acuerdos: '{formato}'.")
        };
    }

    public static IImportadorArchivo<Despacho> CrearPedidos(string formato)
    {
        if (string.IsNullOrWhiteSpace(formato))
        {
            throw new NexoLogException("Debe indicar un formato para pedidos.");
        }

        var tipo = formato.Trim().ToLower();
        return tipo switch
        {
            "json" => new ImportadorPedidosJson(),
            "xml" => new ImportadorPedidosXml(),
            _ => throw new NexoLogException($"Formato no soportado para pedidos: '{formato}'.")
        };
    }

    public static IImportadorArchivo<Acontecimiento> CrearSeguimientos(string formato)
    {
        if (string.IsNullOrWhiteSpace(formato))
        {
            throw new NexoLogException("Debe indicar un formato para seguimientos.");
        }

        var tipo = formato.Trim().ToLower();
        return tipo switch
        {
            "json" => new ImportadorSeguimientosJson(),
            "xml" => new ImportadorSeguimientosXml(),
            _ => throw new NexoLogException($"Formato no soportado para seguimientos: '{formato}'.")
        };
    }
}
