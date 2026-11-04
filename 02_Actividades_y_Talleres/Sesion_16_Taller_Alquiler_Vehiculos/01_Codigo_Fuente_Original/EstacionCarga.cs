using TallerVehiculos.Dominio;

namespace TallerVehiculos.Servicios;

public class EstacionCarga
{
    public void Recargar(PatinetaElectrica vehiculo, int cantidad)
    {
        vehiculo.Cargar(cantidad);
    }
}
