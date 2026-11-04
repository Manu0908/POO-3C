using TallerVehiculos.Contratos;

namespace TallerVehiculos.Dominio;

public class PatinetaElectrica : Vehiculo, IRecargable
{
    private const double CargoFijoActivacion = 2000;
    private const int BateriaMaxima = 100;

    private int _nivelBateria;

    public override double TarifaHora
    {
        get { return 8000; }
    }

    public PatinetaElectrica(string codigo, int nivelBateria) : base(codigo)
    {
        NivelBateria = nivelBateria;
    }

    public int NivelBateria
    {
        get { return _nivelBateria; }
        private set
        {
            if (value < 0 || value > BateriaMaxima)
            {
                throw new ArgumentException($"El nivel de bateria de la patineta debe estar entre 0 y {BateriaMaxima}.");
            }
            _nivelBateria = value;
        }
    }

    public void Recargar(int cantidad)
    {
        if (cantidad <= 0)
        {
            throw new ArgumentException("La cantidad a recargar debe ser mayor que cero.");
        }
        if (_nivelBateria + cantidad > BateriaMaxima)
        {
            throw new ArgumentException($"La patineta {Codigo} esta en {_nivelBateria}% y no puede recibir {cantidad} puntos sin superar el {BateriaMaxima}%.");
        }
        NivelBateria = _nivelBateria + cantidad;
    }

    public override double CalcularValorAlquiler(int horas)
    {
        return base.CalcularValorAlquiler(horas) + CargoFijoActivacion;
    }

    public override void MostrarInfo()
    {
        Console.WriteLine($"[Patineta Electrica] Codigo: {Codigo} | Tarifa: ${TarifaHora:N0}/h + activacion ${CargoFijoActivacion:N0} | Bateria: {NivelBateria}%");
    }
}
