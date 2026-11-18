using NexoLog.DTOs;

namespace NexoLog;

public class Paquete
{
    private string _idPaquete;
    private double _peso;
    private double _valorDeclarado;

    private Paquete() { }

    public Paquete(string idPaquete, string descripcion, double peso, double valorDeclarado)
    {
        IdPaquete = idPaquete;
        Descripcion = descripcion;
        Peso = peso;
        ValorDeclarado = valorDeclarado;
    }

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

    public double ValorDeclarado
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

    public static Paquete MapToDomain(PaqueteDto dto)
    {
        if (dto == null)
        {
            throw new NexoLogException("El DTO del paquete no puede ser nulo.");
        }

        return new Paquete(dto.IdPaquete, dto.Descripcion, dto.Peso, dto.ValorDeclarado);
    }

    public string MostrarInformacion()
    {
        return $"Paquete [{IdPaquete}] - {Descripcion} | Peso: {Peso:F2} kg | Declarado: ${ValorDeclarado:N2}";
    }
}
