using TallerVehiculos.Contratos;

namespace TallerVehiculos.Dominio;

public class PatinetaElectrica : Vehiculo, IRecargable
{
    public int NivelBateria { get; set; }

    public PatinetaElectrica(string codigo) : base(codigo, 8000m)
    {
    }

    public void Cargar(int cantidad)
    {
        NivelBateria += cantidad;
    }
}
