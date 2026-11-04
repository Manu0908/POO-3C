namespace NexoLog.Dominio;

// Excepción personalizada para el control de reglas del negocio en el sistema NexoLog.
public class NexoLogException : Exception
{
    public NexoLogException()
        : base("Ha ocurrido un error en las reglas de negocio de NexoLog.")
    {
    }

    public NexoLogException(string mensaje)
        : base(mensaje)
    {
    }

    public NexoLogException(string mensaje, Exception excepcionInterna)
        : base(mensaje, excepcionInterna)
    {
    }
}
