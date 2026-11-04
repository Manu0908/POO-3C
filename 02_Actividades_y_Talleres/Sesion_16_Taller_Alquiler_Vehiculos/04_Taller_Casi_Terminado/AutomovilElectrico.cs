using TallerVehiculos.Contratos;

namespace TallerVehiculos.Dominio;

public class AutomovilElectrico : Vehiculo, IRecargable
{
    private const int BateriaMaxima = 100;
	private const double Descuento = 0.10;

	private int _nivelBateria;
    private int _cantidadHoras;
    public override double TarifaHora
    {
        get { return 18000; }
    }

    public AutomovilElectrico(string codigo, int cantidadHoras, int nivelBateria) : base(codigo)
    {
        CantidadHoras = cantidadHoras;
		NivelBateria = nivelBateria;
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

	public int NivelBateria
	{
		get { return _nivelBateria; }
		private set
		{
			if (value <= 0 || value > BateriaMaxima)
			{
				throw new Exception($"El nivel de bateria no puede ser negativo y no puede ser mayor que {BateriaMaxima}");
			}
			_nivelBateria = value;
		}
	}

	public override double CalcularValorAlquiler()
    {
        double valor = TarifaHora * CantidadHoras;
        if (CantidadHoras >= 5)
        {
            return valor  - (valor * Descuento);
        }

        return valor;
    }
	public void Recargar(int cantidad)
	{
		if (cantidad <= 0)
		{
			Console.WriteLine("La cantidad a cargar debe ser mayor que cero.");
			return;
		}
		if (_nivelBateria + cantidad > BateriaMaxima)
		{
			Console.WriteLine("No se puede superar el 100% de bateria.");
			return;
		}
		NivelBateria = _nivelBateria + cantidad;
	}

	public override void MostrarInfo()
	{
		Console.WriteLine($"[Automovil Electrico] Codigo: {Codigo} | Horas: {CantidadHoras} | Bateria: {NivelBateria}% | Costo: ${CalcularValorAlquiler():N0}");
	}
}
