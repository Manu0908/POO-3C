namespace NexoLog;

// Excepcion de reglas de negocio para NexoLog.
public class NexoLogException : Exception
{
    public NexoLogException() : base("Ha ocurrido un error en las reglas de negocio de NexoLog.") { }

    public NexoLogException(string mensaje) : base(mensaje) { }

    public NexoLogException(string mensaje, Exception excepcionInterna) : base(mensaje, excepcionInterna) { }
}
