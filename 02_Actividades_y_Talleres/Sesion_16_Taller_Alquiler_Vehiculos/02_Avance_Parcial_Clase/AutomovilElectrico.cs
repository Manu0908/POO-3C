using TallerVehiculos.Contratos;

namespace TallerVehiculos.Dominio;

public class AutomovilElectrico : Vehiculo, IRecargable
{
    public int NivelBateria { get; set; }
    private const double Descuento = 0.10;
    private int _cantidadHoras;
    public override double TarifaHora
    {
        get { return 18000; }
    }

    public AutomovilElectrico(string codigo, int cantidadHoras) : base(codigo)
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

    public override double CalcularCosto()
    {
        double valor = TarifaHora * CantidadHoras;
        if (CantidadHoras > 5)
        {
            return valor  - (valor * Descuento);
        }

        return valor;
    }
	public void Cargar(int cantidad)
    {
        NivelBateria += cantidad;
    }
}
