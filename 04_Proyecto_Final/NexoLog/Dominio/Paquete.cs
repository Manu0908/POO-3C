namespace NexoLog.Dominio;

/// <summary>
/// Representa una unidad física de carga asociada a un despacho.
/// </summary>
public class Paquete
{
    private string _idPaquete;
    private string _descripcion;
    private double _peso;
    private decimal _valorDeclarado;

    public string IdPaquete
    {
        get { return _idPaquete; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NexoLogException("El identificador del paquete es obligatorio y no puede estar vacío.");
            }
            _idPaquete = value.Trim();
        }
    }

    public string Descripcion
    {
        get { return _descripcion; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NexoLogException("La descripción del paquete es obligatoria y no puede estar vacía.");
            }
            _descripcion = value.Trim();
        }
    }

    public double Peso
    {
        get { return _peso; }
        private set
        {
            if (value <= 0)
            {
                throw new NexoLogException("El peso del paquete debe ser un valor positivo mayor a 0 kg.");
            }
            _peso = value;
        }
    }

    public decimal ValorDeclarado
    {
        get { return _valorDeclarado; }
        private set
        {
            if (value < 0)
            {
                throw new NexoLogException("El valor declarado del paquete no puede ser negativo.");
            }
            _valorDeclarado = value;
        }
    }

    public Paquete(string idPaquete, string descripcion, double peso, decimal valorDeclarado)
    {
        IdPaquete = idPaquete;
        Descripcion = descripcion;
        Peso = peso;
        ValorDeclarado = valorDeclarado;
    }

    public override string ToString()
    {
        return $"Paquete [{IdPaquete}] - {Descripcion} | Peso: {Peso:F2} kg | Declarado: ${ValorDeclarado:N2}";
    }
}
