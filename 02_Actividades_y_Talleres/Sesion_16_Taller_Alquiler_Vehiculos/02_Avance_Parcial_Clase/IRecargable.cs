namespace TallerVehiculos.Contratos;

public interface IRecargable
{
    int NivelBateria { get; }
    void Cargar(int cantidad);
    decimal CalcularCosto(int horas);
}
