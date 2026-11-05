namespace NexoLog;

// Representa un evento o hito registrado en el seguimiento de un despacho.
public class Acontecimiento
{
    // Constantes
    public const string TipoRec = "REC";
    public const string TipoRut = "RUT";
    public const string TipoEnt = "ENT";
    public const string TipoDev = "DEV";

    // Constructor horizontal
    public Acontecimiento(string idAcontecimiento, string numeroGuia, string tipo, DateTime fechaHora)
    {
        IdAcontecimiento = idAcontecimiento;
        NumeroGuia = numeroGuia;
        Tipo = tipo != null ? tipo.Trim().ToUpperInvariant() : string.Empty;
        FechaHora = fechaHora;
    }

    // Propiedades publicas
    public string IdAcontecimiento { get; private set; }
    public string NumeroGuia { get; private set; }
    public string Tipo { get; private set; }
    public DateTime FechaHora { get; private set; }

    // Metodos
    public string MostrarInformacion()
    {
        return $"[{FechaHora:yyyy-MM-dd HH:mm:ss}] Guia: {NumeroGuia} | Tipo: {Tipo} | ID: {IdAcontecimiento}";
    }
}
