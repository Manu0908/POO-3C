namespace NexoLog.Dominio;

// Representa un evento o hito registrado en la línea de tiempo de un despacho.
public class Acontecimiento
{
    public const string TipoRec = "REC";
    public const string TipoRut = "RUT";
    public const string TipoEnt = "ENT";
    public const string TipoDev = "DEV";

    private string _idAcontecimiento;
    private string _numeroGuia;
    private string _tipo;
    private DateTime _fechaHora;

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
                throw new NexoLogException("El número de guía asociado al acontecimiento es obligatorio.");
            }
            _numeroGuia = value.Trim();
        }
    }

    public string Tipo
    {
        get { return _tipo; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NexoLogException("El tipo de acontecimiento es obligatorio.");
            }
            _tipo = value.Trim().ToUpperInvariant();
        }
    }

    public DateTime FechaHora
    {
        get { return _fechaHora; }
        private set { _fechaHora = value; }
    }

    public Acontecimiento(string idAcontecimiento, string numeroGuia, string tipo, DateTime fechaHora)
    {
        IdAcontecimiento = idAcontecimiento;
        NumeroGuia = numeroGuia;
        Tipo = tipo;
        FechaHora = fechaHora;
    }

    public string MostrarInformacion()
    {
        return $"[{FechaHora:yyyy-MM-dd HH:mm:ss}] Guía: {NumeroGuia} | Tipo: {Tipo} | ID: {IdAcontecimiento}";
    }
}
