using NexoLog.Dominio;
using NexoLog.Infraestructura.Lectores.DTOs;

namespace NexoLog.Infraestructura.Lectores;

// Fábrica para centralizar la instanciación de lectores de archivo.
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
