namespace NexoLog.Servicios;

public abstract class ModalidadServicio
{
    protected ModalidadServicio(string nombreModalidad)
    {
        NombreModalidad = nombreModalidad;
    }

    public string NombreModalidad { get; private set; }

    public abstract bool EvaluarCumplimiento(Despacho despacho);

    public string MostrarInformacion()
    {
        return NombreModalidad;
    }
}
