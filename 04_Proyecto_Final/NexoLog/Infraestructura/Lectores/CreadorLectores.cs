using NexoLog.Dominio;
using NexoLog.Infraestructura.Lectores.DTOs;

namespace NexoLog.Infraestructura.Lectores;

// Creador centralizado de instancias de lectores de archivo.
public static class CreadorLectores
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
