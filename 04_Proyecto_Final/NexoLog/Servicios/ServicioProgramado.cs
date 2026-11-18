namespace NexoLog.Servicios;

public class ServicioProgramado : ModalidadServicio
{
    private DateTime _fechaProgramada;
    private int _horaInicio;
    private int _horaFin;

    public override string NombreModalidad { get { return "Programado"; } }

    private ServicioProgramado() { }

    public ServicioProgramado(DateTime fechaProgramada, int horaInicio, int horaFin)
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

        Acontecimiento eventoRut = despacho.BuscarAcontecimiento(Acontecimiento.TipoRut);
        Acontecimiento eventoEnt = despacho.BuscarAcontecimiento(Acontecimiento.TipoEnt);
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
        return horaEntrega >= _horaInicio && horaEntrega < _horaFin;
    }
}
