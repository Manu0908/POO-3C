namespace NexoLog.Servicios;

public class ServicioProgramado : ModalidadServicio
{
    private DateTime _fechaProgramada;
    private int _horaInicio;
    private int _horaFin;

    public ServicioProgramado(DateTime fechaProgramada, int horaInicio, int horaFin) : base("Programado")
    {
        if (horaFin <= horaInicio)
        {
            throw new NexoLogException("La hora final de la franja horaria debe ser estrictamente posterior a la hora inicial.");
        }
        _fechaProgramada = fechaProgramada.Date;
        _horaInicio = horaInicio;
        _horaFin = horaFin;
    }

    public DateTime FechaProgramada { get { return _fechaProgramada; } }
    public int HoraInicio { get { return _horaInicio; } }
    public int HoraFin { get { return _horaFin; } }

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
            if (acontecimiento.Tipo == Acontecimiento.TipoRut && eventoRut == null)
            {
                eventoRut = acontecimiento;
            }
            else if (acontecimiento.Tipo == Acontecimiento.TipoEnt)
            {
                eventoEnt = acontecimiento;
            }
        }

        if (eventoEnt == null)
        {
            return false;
        }

        if (eventoRut != null && eventoRut.FechaHora.Date < _fechaProgramada.Date)
        {
            return false;
        }

        if (eventoEnt.FechaHora.Date != _fechaProgramada.Date)
        {
            return false;
        }

        int horaEntrega = eventoEnt.FechaHora.Hour;
        if (horaEntrega >= _horaInicio && horaEntrega < _horaFin)
        {
            return true;
        }

        return false;
    }
}
