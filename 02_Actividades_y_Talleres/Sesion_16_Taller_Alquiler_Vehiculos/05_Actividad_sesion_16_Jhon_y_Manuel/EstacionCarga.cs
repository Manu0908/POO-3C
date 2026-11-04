using TallerVehiculos.Contratos;
using TallerVehiculos.Dominio;

namespace TallerVehiculos.Servicios;

public class EstacionCarga
{
    public void Recargar(IRecargable vehiculo, int cantidad)
    {
        if (vehiculo == null)
        {
            throw new ArgumentException("No se puede recargar un vehiculo nulo.");
        }
        vehiculo.Recargar(cantidad);
    }
}
