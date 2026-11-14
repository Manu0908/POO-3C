namespace NexoLog;

// Representa una unidad fisica de carga asociada a un despacho.
public class Paquete
{
    // Campos privados con validacion
    private string _idPaquete;
    private double _peso;
    private decimal _valorDeclarado;

    // Constructor privado sin parametros para serializacion / reflexion
    private Paquete() { }

    // Constructor horizontal
    public Paquete(string idPaquete, string descripcion, double peso, decimal valorDeclarado)
    {
        IdPaquete = idPaquete;
        Descripcion = descripcion;
        Peso = peso;
        ValorDeclarado = valorDeclarado;
    }

    // Propiedades publicas
    public string Descripcion { get; private set; }

    public string IdPaquete
    {
        get { return _idPaquete; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NexoLogException("El identificador del paquete es obligatorio.");
            }
            _idPaquete = value.Trim();
        }
    }

    public decimal ValorDeclarado
    {
        get { return _valorDeclarado; }
        private set
        {
            if (value < 0m)
            {
                throw new NexoLogException("El valor declarado del paquete no puede ser negativo.");
            }
            _valorDeclarado = value;
        }
    }

    public double Peso
    {
        get { return _peso; }
        private set
        {
            if (value <= 0)
            {
                throw new NexoLogException("El peso del paquete debe ser estrictamente mayor a 0 kg.");
            }
            _peso = value;
        }
    }

    // Metodos
    public string MostrarInformacion()
    {
        return $"Paquete [{IdPaquete}] - {Descripcion} | Peso: {Peso:F2} kg | Declarado: ${ValorDeclarado:N2}";
    }
}
