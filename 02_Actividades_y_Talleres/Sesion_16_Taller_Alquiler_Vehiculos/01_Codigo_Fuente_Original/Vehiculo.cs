namespace TallerVehiculos.Dominio;

public class Vehiculo
{
    public string Codigo { get; set; }
    public decimal TarifaHora { get; set; }

    public Vehiculo(string codigo, decimal tarifaHora)
    {
        Codigo = codigo;
        TarifaHora = tarifaHora;
    }

    public decimal CalcularCosto(int horas)
    {
        return TarifaHora * horas;
    }
}
