namespace NexoLog.Dominio;

/// <summary>
/// Representa un evento o hito registrado en la línea de tiempo de un despacho.
/// </summary>
public class Acontecimiento
{
    private string _idAcontecimiento;
    private string _numeroGuia;
    private TipoAcontecimiento _tipo;
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

    public TipoAcontecimiento Tipo
    {
        get { return _tipo; }
        private set { _tipo = value; }
    }

    public DateTime FechaHora
    {
        get { return _fechaHora; }
        private set { _fechaHora = value; }
    }

    public Acontecimiento(string idAcontecimiento, string numeroGuia, TipoAcontecimiento tipo, DateTime fechaHora)
    {
        IdAcontecimiento = idAcontecimiento;
        NumeroGuia = numeroGuia;
        Tipo = tipo;
        FechaHora = fechaHora;
    }

    public override string ToString()
    {
        return $"[{FechaHora:yyyy-MM-dd HH:mm:ss}] Guía: {NumeroGuia} | Tipo: {Tipo} | ID: {IdAcontecimiento}";
    }
}
