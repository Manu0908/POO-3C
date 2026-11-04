using TallerVehiculos.Contratos;

namespace TallerVehiculos.Dominio;

public class PatinetaElectrica : Vehiculo, IRecargable
{
    private const double CargoFijoActivación = 2000;
    private int _cantidadHoras;
    public int NivelBateria { get; set; }
    public override double TarifaHora
    {
        get { return 8000; }
    }

    public PatinetaElectrica(string codigo, int cantidadHoras) : base(codigo)
    {
        CantidadHoras = cantidadHoras;
    }

	public int CantidadHoras
    {
        get { return _cantidadHoras; }
        private set
        {
            if (value <= 0)
            {
                throw new Exception("Las horas deben ser positivas");
            }
            _cantidadHoras = value;
        }
    }


	public void Cargar(int cantidad)
    {
        NivelBateria += cantidad;
    }

    public override double CalcularCosto()
    {
        return (TarifaHora * CantidadHoras) + CargoFijoActivación;
    }
}
