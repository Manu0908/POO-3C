namespace NexoLog.Servicios;

public abstract class ModalidadServicio
{
    public abstract string NombreModalidad { get; }

    protected ModalidadServicio() { }

    public abstract bool EvaluarCumplimiento(Despacho despacho);

    public string MostrarInformacion()
    {
        return NombreModalidad;
    }

    public static ModalidadServicio Crear(string modalidad, DateTime? inicio, DateTime? fin, string numeroGuia)
    {
        string tipo = (modalidad ?? string.Empty).Trim().ToUpper();
        return tipo switch
        {
            "ESTANDAR" => new ServicioEstandar(),
            "ESTÁNDAR" => new ServicioEstandar(),
            "EXPRES" => new ServicioExpres(),
            "EXPRÉS" => new ServicioExpres(),
            "PROGRAMADO" => CrearProgramado(inicio, fin, numeroGuia),
            _ => throw new NexoLogException($"Modalidad de servicio desconocida '{modalidad}' en la guia '{numeroGuia}'.")
        };
    }

    private static ModalidadServicio CrearProgramado(DateTime? inicio, DateTime? fin, string numeroGuia)
    {
        if (!inicio.HasValue || !fin.HasValue)
        {
            throw new NexoLogException($"Falta la programacion de inicio o fin en la guia '{numeroGuia}'.");
        }

        return new ServicioProgramado(inicio.Value, fin.Value);
    }
}
