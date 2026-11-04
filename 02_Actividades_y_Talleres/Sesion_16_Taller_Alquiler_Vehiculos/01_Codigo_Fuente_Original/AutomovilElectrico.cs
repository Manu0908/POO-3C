using TallerVehiculos.Contratos;

namespace TallerVehiculos.Dominio;

public class AutomovilElectrico : Vehiculo, IRecargable
{
    public int NivelBateria { get; set; }

    public AutomovilElectrico(string codigo) : base(codigo, 18000m)
    {
    }

    public void Cargar(int cantidad)
    {
        NivelBateria += cantidad;
    }
}
