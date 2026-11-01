using NexoLog.Dominio;
using NexoLog.Infraestructura.Lectores.DTOs;

namespace NexoLog.Infraestructura.Lectores;

/// <summary>
/// Fábrica centralizada (Factory) para la obtención de lectores concretos de archivos.
/// </summary>
public static class LectorArchivoFactory
{
    public static ILectorArchivo<AcuerdoDto> CrearLectorAcuerdosJson()
    {
        return new LectorAcuerdosJson();
    }

    public static ILectorArchivo<Despacho> CrearLectorPedidosJson()
    {
        return new LectorPedidosJson();
    }

    public static ILectorArchivo<Despacho> CrearLectorPedidosXml()
    {
        return new LectorPedidosXml();
    }

    public static ILectorArchivo<Acontecimiento> CrearLectorSeguimientosJson()
    {
        return new LectorSeguimientosJson();
    }

    public static ILectorArchivo<Acontecimiento> CrearLectorSeguimientosXml()
    {
        return new LectorSeguimientosXml();
    }
}
