using TallerVehiculos.Contratos;

namespace TallerVehiculos.Dominio;

public class PatinetaElectrica : Vehiculo, IRecargable
{
    private const double CargoFijoActivacion = 2000;
    private int _cantidadHoras;
    private int _nivelBateria;

    public override double TarifaHora
    {
        get { return 9000; }
    }

    public int CantidadHoras
    {
        get { return _cantidadHoras; }
        private set
        {
            if (value <= 0)
            {
                throw new Exception("Las horas deben ser positivas.");
            }
            _cantidadHoras = value;
        }
    }

    public int NivelBateria
    {
        get { return _nivelBateria; }
        private set
        {
            if (value < 0)
            {
                throw new Exception("El nivel de bateria no puede ser negativo.");
            }
            if (value > 100)
            {
                throw new Exception("El nivel de bateria no puede superar el 100%.");
            }
            _nivelBateria = value;
        }
    }

    public PatinetaElectrica(string codigo, int cantidadHoras) : base(codigo)
    {
        CantidadHoras = cantidadHoras;
        NivelBateria = 0;
    }

    public void Cargar(int cantidad)
    {
        if (cantidad <= 0)
        {
            Console.WriteLine("La cantidad a cargar debe ser mayor que cero.");
            return;
        }
        if (_nivelBateria + cantidad > 100)
        {
            Console.WriteLine("No se puede superar el 100% de bateria.");
            return;
        }
        NivelBateria = _nivelBateria + cantidad;
    }

    public override double CalcularCosto()
    {
        return (TarifaHora * CantidadHoras) + CargoFijoActivacion;
    }

    public override void MostrarInfo()
    {
        Console.WriteLine($"[Patineta Electrica] Codigo: {Codigo} | Horas: {CantidadHoras} | Bateria: {NivelBateria}% | Costo: ${CalcularCosto():N0}");
    }
}

