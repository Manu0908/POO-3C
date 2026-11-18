namespace NexoLog.Servicios;

public abstract class ModalidadServicio
{
    public abstract string NombreModalidad { get; }

    protected ModalidadServicio() { }

    public abstract bool Cumple(Despacho despacho);

    public string MostrarInformacion()
    {
        return NombreModalidad;
    }

    public static ModalidadServicio Crear(string modalidad, DateTime? inicio, DateTime? fin, string numeroGuia)
    {
        if (string.IsNullOrWhiteSpace(modalidad))
        {
            throw new NexoLogException($"Modalidad de servicio desconocida '{modalidad}' en la guia '{numeroGuia}'.");
        }

        string tipo = modalidad.Trim().ToUpper();
        return tipo switch
        {
            "ESTANDAR" => new ServicioEstandar(),
            "EXPRES" => new ServicioExpres(),
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
