namespace NexoLog;

// Representa un evento o hito registrado en el seguimiento de un despacho.
public class Acontecimiento
{
    // Constantes
    public const string TipoRec = "REC";
    public const string TipoRut = "RUT";
    public const string TipoEnt = "ENT";
    public const string TipoDev = "DEV";

    // Campos privados con validacion
    private string _idAcontecimiento;
    private string _numeroGuia;

    // Constructor privado sin parametros para serializacion / reflexion
    private Acontecimiento() { }

    // Constructor horizontal
    public Acontecimiento(string idAcontecimiento, string numeroGuia, string tipo, DateTime fechaHora)
    {
        IdAcontecimiento = idAcontecimiento;
        NumeroGuia = numeroGuia;
        Tipo = tipo != null ? tipo.Trim().ToUpperInvariant() : string.Empty;
        FechaHora = fechaHora;
    }

    // Propiedades publicas
    public string Tipo { get; private set; }
    public DateTime FechaHora { get; private set; }

    public string IdAcontecimiento
    {
        get { return _idAcontecimiento; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NexoLogException("El identificador del acontecimiento es obligatorio.");
            }
            _idAcontecimiento = value.Trim();
        }
    }

    public string NumeroGuia
    {
        get { return _numeroGuia; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NexoLogException("El numero de guia asociado al acontecimiento es obligatorio.");
            }
            _numeroGuia = value.Trim();
        }
    }

    // Metodos
    public string MostrarInformacion()
    {
        return $"[{FechaHora:yyyy-MM-dd HH:mm:ss}] Guia: {NumeroGuia} | Tipo: {Tipo} | ID: {IdAcontecimiento}";
    }
}
