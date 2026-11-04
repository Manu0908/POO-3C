namespace TallerVehiculos.Dominio;

public abstract class Vehiculo
{
    private string _codigo;
    public abstract double TarifaHora { get; }

    protected Vehiculo(string codigo)
    {
		_codigo = codigo;
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
        }
    }

    public abstract double CalcularValorAlquiler();

    public abstract void MostrarInfo();
}
