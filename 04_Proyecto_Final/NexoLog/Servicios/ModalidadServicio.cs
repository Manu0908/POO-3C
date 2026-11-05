namespace NexoLog.Servicios;

// Clase base abstracta para las modalidades de servicio y evaluacion de acuerdos de nivel de servicio (SLA).
public abstract class ModalidadServicio
{
    // Constructor horizontal
    protected ModalidadServicio(string nombreModalidad)
    {
        NombreModalidad = nombreModalidad;
    }

    // Propiedades publicas
    public string NombreModalidad { get; private set; }

    // Metodos abstractos y de consulta
    public abstract bool EvaluarCumplimiento(Despacho despacho);

    public string MostrarInformacion()
    {
        return NombreModalidad;
    }
}
