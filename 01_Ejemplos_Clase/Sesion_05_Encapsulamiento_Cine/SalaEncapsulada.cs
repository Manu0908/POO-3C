namespace prueba;

public class SalaEncapsulada
{
    private static int PorcentajeMaximoOcupacion = 90;

    private int _numero;
    private int _capacidadMaxima;
    private int _cantidadActual;

    public int Numero
    {
        get { return _numero; }
        private set
        {
            if (value <= 0)
            {
                throw new Exception("El número debe ser mayor que 0.");
            }
            _numero = value;
        }
    }

    public int CapacidadMaxima
    {
        get { return _capacidadMaxima; }
        private set
        {
            if (value <= 0)
            {
                throw new Exception("La capacidad máxima debe ser mayor que 0.");
            }
            _capacidadMaxima = value;
        }
    }

    private int LimiteOcupacion
    {
        get { return _capacidadMaxima * PorcentajeMaximoOcupacion / 100; }
    }

    public bool Disponible
    {
        get { return _cantidadActual < LimiteOcupacion; }
    }

    public int PuestosDisponibles
    {
        get { return LimiteOcupacion - _cantidadActual; }
    }

    private SalaEncapsulada() { }

    public SalaEncapsulada(int numero, int capacidadMaxima)
    {
        Numero = numero;
        CapacidadMaxima = capacidadMaxima;
        _cantidadActual = 0;
    }

    public void RegistrarIngreso()
    {
        if (Disponible)
        {
            _cantidadActual++;
        }
        else
        {
            Console.WriteLine("No se puede registrar el ingreso, se alcanzó el límite de ocupación.");
        }
    }

    public void RegistrarSalida()
    {
        if (_cantidadActual <= 0)
        {
            Console.WriteLine("No se puede registrar la salida, no hay personas en la sala.");
        }
        else
        {
            _cantidadActual--;
        }
    }
}
