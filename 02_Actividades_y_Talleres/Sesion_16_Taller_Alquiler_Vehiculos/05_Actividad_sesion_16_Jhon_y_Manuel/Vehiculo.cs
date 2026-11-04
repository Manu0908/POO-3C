namespace TallerVehiculos.Dominio;

public abstract class Vehiculo
{
    private string _codigo;

    public abstract double TarifaHora { get; }

    protected Vehiculo(string codigo)
    {
        Codigo = codigo;
    }

    public string Codigo
    {
        get { return _codigo; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("El codigo es obligatorio.");
            }
            _codigo = value.Trim();
        }
    }

    public virtual double CalcularValorAlquiler(int horas)
    {
        if (horas <= 0)
        {
            throw new ArgumentException("Las horas de alquiler deben ser mayores que cero.");
        }
        return TarifaHora * horas;
    }

    public abstract void MostrarInfo();
}
