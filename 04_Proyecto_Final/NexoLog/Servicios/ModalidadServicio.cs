using System.Globalization;

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

    public static ModalidadServicio Crear(string modalidad, string fechaProgramada, string horaInicio, string horaFin, string numeroGuia)
    {
        string tipo = (modalidad ?? string.Empty).Trim().ToUpper();
        return tipo switch
        {
            "ESTANDAR" => new ServicioEstandar(),
            "ESTÁNDAR" => new ServicioEstandar(),
            "EXPRES" => new ServicioExpres(),
            "EXPRÉS" => new ServicioExpres(),
            "PROGRAMADO" => CrearProgramado(fechaProgramada, horaInicio, horaFin, numeroGuia),
            _ => throw new NexoLogException($"Modalidad de servicio desconocida '{modalidad}' en la guia '{numeroGuia}'.")
        };
    }

    private static ModalidadServicio CrearProgramado(string fechaProgramada, string horaInicio, string horaFin, string numeroGuia)
    {
        DateTime fechaProg;
        if (!DateTime.TryParse(fechaProgramada, CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaProg))
        {
            throw new NexoLogException($"Fecha programada invalida '{fechaProgramada}' en la guia '{numeroGuia}'.");
        }

        string parteIni = (horaInicio ?? string.Empty).Split(':')[0].Trim();
        string parteFin = (horaFin ?? string.Empty).Split(':')[0].Trim();

        int horaInicial;
        if (!int.TryParse(parteIni, out horaInicial) || horaInicial < 0 || horaInicial > 23)
        {
            throw new NexoLogException($"Hora inicial programada invalida '{horaInicio}' en la guia '{numeroGuia}'.");
        }

        int horaFinal;
        if (!int.TryParse(parteFin, out horaFinal) || horaFinal < 0 || horaFinal > 24)
        {
            throw new NexoLogException($"Hora final programada invalida '{horaFin}' en la guia '{numeroGuia}'.");
        }

        return new ServicioProgramado(fechaProg, horaInicial, horaFinal);
    }
}
