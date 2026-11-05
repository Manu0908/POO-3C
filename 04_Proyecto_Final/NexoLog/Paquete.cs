namespace NexoLog;

// Representa una unidad fisica de carga asociada a un despacho.
public class Paquete
{
    // Campos privados con validacion
    private double _peso;

    // Constructor horizontal
    public Paquete(string idPaquete, string descripcion, double peso, decimal valorDeclarado)
    {
        IdPaquete = idPaquete;
        Descripcion = descripcion;
        Peso = peso;
        ValorDeclarado = valorDeclarado;
    }

    // Propiedades publicas
    public string IdPaquete { get; private set; }
    public string Descripcion { get; private set; }
    public decimal ValorDeclarado { get; private set; }

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
