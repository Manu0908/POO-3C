namespace NexoLog.Dominio.Modalidades;

/// <summary>
/// Clase base abstracta que define las reglas de cumplimiento (SLA) para las modalidades de servicio.
/// </summary>
public abstract class ModalidadServicio
{
    private string _nombreModalidad;

    public string NombreModalidad
    {
        get { return _nombreModalidad; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NexoLogException("El nombre de la modalidad de servicio es obligatorio.");
            }
            _nombreModalidad = value.Trim();
        }
    }

    protected ModalidadServicio(string nombreModalidad)
    {
        NombreModalidad = nombreModalidad;
    }

    /// <summary>
    /// Evalúa si el despacho cumplió los acuerdos de nivel de servicio específicos de esta modalidad.
    /// </summary>
    public abstract bool EvaluarCumplimiento(Despacho despacho);

    public override string ToString()
    {
        return _nombreModalidad;
    }
}
