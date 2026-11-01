namespace prueba;

public class Sala
{    
    private static int PorcentajeMaximoOcupacion = 90;
    public int Numero { get; private set; }
    public int CapacidadMaxima { get; private set; }
    public int CantidadActual { get; private set; }
    public int LimiteOcupacion { get { return CapacidadMaxima * PorcentajeMaximoOcupacion / 100; } }
    public bool Disponible { get { return CantidadActual < LimiteOcupacion; } }
    public int PuestosDisponibles { get { return LimiteOcupacion - CantidadActual; } }

    private Sala() { }

    public Sala(int numero, int capacidadMaxima)
    {
        Numero = numero;
        CapacidadMaxima = capacidadMaxima;
        CantidadActual = 0;
    }

    public void RegistrarIngreso()
    {
        if (Disponible)
        {
            CantidadActual++;
        }
        else 
        {
            Console.WriteLine("No se puede registrar el ingreso, se alcanzó el límite de ocupación.");
        }
    }

    public void RegistrarSalida()
    {
        if (CantidadActual <= 0)
        {
            Console.WriteLine("No se puede registrar la salida, no hay personas en la sala.");
        }
        else 
        {
            CantidadActual--;
        }
    }
}
