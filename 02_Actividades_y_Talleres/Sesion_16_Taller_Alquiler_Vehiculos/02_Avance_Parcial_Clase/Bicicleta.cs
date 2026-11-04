namespace TallerVehiculos.Dominio;

public class Bicicleta : Vehiculo
{
    private int _cantidadHoras;

    public override double TarifaHora
    {
        get { return 5000; }
    }
    public Bicicleta(string codigo, int cantidadHoras) : base(codigo)
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
        return TarifaHora * CantidadHoras;
    }
}

