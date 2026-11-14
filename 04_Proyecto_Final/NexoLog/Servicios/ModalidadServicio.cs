using System.Globalization;
using NexoLog;

namespace NexoLog.Servicios;

public abstract class ModalidadServicio
{
    protected ModalidadServicio() { }

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

    public static ModalidadServicio Crear(string modalidad, string fechaProgramada, string horaInicio, string horaFin, string numeroGuia)
    {
        string modalidadTexto = (modalidad ?? string.Empty).Trim().ToUpperInvariant();
        switch (modalidadTexto)
        {
            case "ESTANDAR":
            case "ESTÁNDAR":
                return new ServicioEstandar();

            case "EXPRES":
            case "EXPRÉS":
                return new ServicioExpres();

            case "PROGRAMADO":
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

                try
                {
                    return new ServicioProgramado(fechaProg, horaInicial, horaFinal);
                }
                catch (NexoLogException ex)
                {
                    throw new NexoLogException($"Modalidad invalida en la guia '{numeroGuia}': {ex.Message}");
                }

            default:
                throw new NexoLogException($"Modalidad de servicio desconocida '{modalidad}' en la guia '{numeroGuia}'.");
        }
    }
}
