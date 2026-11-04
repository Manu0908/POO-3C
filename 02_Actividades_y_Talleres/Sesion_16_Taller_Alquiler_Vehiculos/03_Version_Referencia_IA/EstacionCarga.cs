using TallerVehiculos.Contratos;

namespace TallerVehiculos.Servicios;

public class EstacionCarga
{
    public void Recargar(IRecargable vehiculo, int cantidad)
    {
        vehiculo.Cargar(cantidad);
    }
}

