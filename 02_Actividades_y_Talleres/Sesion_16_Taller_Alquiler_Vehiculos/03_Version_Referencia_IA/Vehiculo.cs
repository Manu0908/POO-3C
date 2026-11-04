namespace TallerVehiculos.Dominio;

public abstract class Vehiculo
{
    private string _codigo;

    public string Codigo
    {
        get { return _codigo; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new Exception("El codigo es obligatorio.");
            }
            _codigo = value.Trim();
        }
    }

    public abstract double TarifaHora { get; }

    protected Vehiculo(string codigo)
    {
        Codigo = codigo;
    }

    public abstract double CalcularCosto();

    public abstract void MostrarInfo();
}
