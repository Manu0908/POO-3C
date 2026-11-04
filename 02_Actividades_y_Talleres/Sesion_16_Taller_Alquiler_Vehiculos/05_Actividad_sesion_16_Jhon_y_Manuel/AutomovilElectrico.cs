using TallerVehiculos.Contratos;

namespace TallerVehiculos.Dominio;

public class AutomovilElectrico : Vehiculo, IRecargable
{
    private const double Descuento = 0.10;
    private const int HorasMinimasDescuento = 5;
    private const int BateriaMaxima = 100;

    private int _nivelBateria;

    public override double TarifaHora
    {
        get { return 18000; }
    }

    public AutomovilElectrico(string codigo, int nivelBateria) : base(codigo)
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
                throw new ArgumentException($"El nivel de bateria del automovil debe estar entre 0 y {BateriaMaxima}.");
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
            throw new ArgumentException($"El automovil {Codigo} esta en {_nivelBateria}% y no puede recibir {cantidad} puntos sin superar el {BateriaMaxima}%.");
        }
        NivelBateria = _nivelBateria + cantidad;
    }

    public override double CalcularValorAlquiler(int horas)
    {
        double valor = base.CalcularValorAlquiler(horas);
        if (horas >= HorasMinimasDescuento)
        {
            return valor - (valor * Descuento);
        }
        return valor;
    }

    public override void MostrarInfo()
    {
        Console.WriteLine($"[Automovil Electrico] Codigo: {Codigo} | Tarifa: ${TarifaHora:N0}/h (10% dto. desde {HorasMinimasDescuento} h) | Bateria: {NivelBateria}%");
    }
}
