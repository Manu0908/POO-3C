using TallerVehiculos.Contratos;
using TallerVehiculos.Dominio;

namespace TallerVehiculos.Servicios;

public class EstacionCarga
{
    public void Recargar(IRecargable vehiculo, int cantidad)
    {
        vehiculo.Recargar(cantidad);
    }
}
