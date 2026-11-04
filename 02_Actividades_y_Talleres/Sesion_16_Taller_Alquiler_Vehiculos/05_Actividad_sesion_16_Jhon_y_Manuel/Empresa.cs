using TallerVehiculos.Dominio;

namespace TallerVehiculos.Servicios;

public class Empresa
{
    private List<Vehiculo> _vehiculos;

    public Empresa()
    {
        _vehiculos = new List<Vehiculo>();
    }

    public List<Vehiculo> Vehiculos { get { return _vehiculos; } }

    public void IncorporarVehiculo(Vehiculo vehiculo)
    {
        if (vehiculo == null)
        {
            throw new ArgumentException("No se puede incorporar un vehiculo nulo.");
        }
        if (BuscarPorCodigo(vehiculo.Codigo) != null)
        {
            throw new Exception($"Ya existe un vehiculo con el codigo {vehiculo.Codigo}.");
        }
        _vehiculos.Add(vehiculo);
    }

    public void RetirarVehiculo(string codigo)
    {
        Vehiculo encontrado = BuscarPorCodigo(codigo);
        if (encontrado == null)
        {
            throw new Exception($"No existe un vehiculo con el codigo {codigo}.");
        }
        _vehiculos.Remove(encontrado);
    }

    public void MostrarFlota()
    {
        if (_vehiculos.Count == 0)
        {
            Console.WriteLine("La flota esta vacia.");
            return;
        }

        foreach (Vehiculo vehiculo in _vehiculos)
        {
            vehiculo.MostrarInfo();
        }
    }

    public double CalcularCostoTotal(int horas)
    {
        double total = 0;
        foreach (Vehiculo vehiculo in _vehiculos)
        {
            total += vehiculo.CalcularValorAlquiler(horas);
        }
        return total;
    }

    private Vehiculo BuscarPorCodigo(string codigo)
    {
        foreach (Vehiculo vehiculo in _vehiculos)
        {
            if (vehiculo.Codigo == codigo)
            {
                return vehiculo;
            }
        }
        return null;
    }
}
