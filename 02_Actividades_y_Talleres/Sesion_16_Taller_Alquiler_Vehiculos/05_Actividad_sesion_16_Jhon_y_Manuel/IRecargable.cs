namespace TallerVehiculos.Contratos;

public interface IRecargable
{
    int NivelBateria { get; }

    void Recargar(int cantidad);
}
