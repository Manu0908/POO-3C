namespace TallerVehiculos.Dominio;

public class Bicicleta : Vehiculo
{
    public override double TarifaHora
    {
        get { return 5000; }
    }

    public Bicicleta(string codigo) : base(codigo)
    {
    }

    public override void MostrarInfo()
    {
        Console.WriteLine($"[Bicicleta] Codigo: {Codigo} | Tarifa: ${TarifaHora:N0}/h");
    }
}
