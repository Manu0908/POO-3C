namespace NexoLog.Dominio.Modalidades;

// Modalidad Programado: entrega en fecha pactada y en franja horaria pactada.
public class ServicioProgramado : ModalidadServicio
{
    private DateTime _fechaProgramada;
    private TimeSpan _horaInicio;
    private TimeSpan _horaFin;

    public DateTime FechaProgramada
    {
        get { return _fechaProgramada; }
        private set { _fechaProgramada = value.Date; }
    }

    public TimeSpan HoraInicio
    {
        get { return _horaInicio; }
        private set { _horaInicio = value; }
    }

    public TimeSpan HoraFin
    {
        get { return _horaFin; }
        private set { _horaFin = value; }
    }

    public ServicioProgramado(DateTime fechaProgramada, TimeSpan horaInicio, TimeSpan horaFin)
        : base("Programado")
    {
        if (horaFin <= horaInicio)
        {
            throw new NexoLogException("La hora final de la franja horaria debe ser estrictamente posterior a la hora inicial.");
        }

        FechaProgramada = fechaProgramada;
        HoraInicio = horaInicio;
        HoraFin = horaFin;
    }

    public override bool EvaluarCumplimiento(Despacho despacho)
    {
        if (despacho == null)
        {
            return false;
        }

        Acontecimiento eventoRut = null;
        Acontecimiento eventoEnt = null;

        foreach (Acontecimiento acontecimiento in despacho.Historial)
        {
            if (string.Equals(acontecimiento.Tipo, Acontecimiento.TipoRut, StringComparison.OrdinalIgnoreCase) && eventoRut == null)
            {
                eventoRut = acontecimiento;
            }
            else if (string.Equals(acontecimiento.Tipo, Acontecimiento.TipoEnt, StringComparison.OrdinalIgnoreCase))
            {
                eventoEnt = acontecimiento;
            }
        }

        if (eventoEnt == null)
        {
            return false;
        }

        // Condición 1: No debe salir a ruta antes del día programado
        if (eventoRut != null && eventoRut.FechaHora.Date < _fechaProgramada.Date)
        {
            return false;
        }

        // Condición 2: La entrega debe realizarse exactamente en la fecha pactada
        if (eventoEnt.FechaHora.Date != _fechaProgramada.Date)
        {
            return false;
        }

        // Condición 3: La hora de entrega debe situarse dentro de la franja horaria pactada
        TimeSpan horaEntrega = eventoEnt.FechaHora.TimeOfDay;
        if (horaEntrega < _horaInicio || horaEntrega > _horaFin)
        {
            return false;
        }

        return true;
    }
}
