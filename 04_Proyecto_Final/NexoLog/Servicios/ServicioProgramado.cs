namespace NexoLog.Servicios;

// Modalidad Programado: entrega en fecha pactada y dentro de la franja horaria acordada.
public class ServicioProgramado : ModalidadServicio
{
    // Campos privados
    private DateTime _fechaProgramada;
    private int _horaInicio;
    private int _horaFin;

    // Constructor horizontal
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

    // Propiedades publicas
    public DateTime FechaProgramada { get { return _fechaProgramada; } }
    public int HoraInicio { get { return _horaInicio; } }
    public int HoraFin { get { return _horaFin; } }

    // Evaluacion de SLA
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
            if (acontecimiento.Tipo.ToLower() == Acontecimiento.TipoRut.ToLower() && eventoRut == null)
            {
                eventoRut = acontecimiento;
            }
            else if (acontecimiento.Tipo.ToLower() == Acontecimiento.TipoEnt.ToLower())
            {
                eventoEnt = acontecimiento;
            }
        }

        if (eventoEnt == null)
        {
            return false;
        }

        // Condicion 1: No salir a ruta antes del dia programado
        if (eventoRut != null && eventoRut.FechaHora.Date < _fechaProgramada.Date)
        {
            return false;
        }

        // Condicion 2: La entrega debe realizarse en la fecha pactada
        if (eventoEnt.FechaHora.Date != _fechaProgramada.Date)
        {
            return false;
        }

        // Condicion 3: La hora de entrega debe situarse dentro de la franja horaria
        int horaEntrega = eventoEnt.FechaHora.Hour;
        if (horaEntrega >= _horaInicio && horaEntrega < _horaFin)
        {
            return true;
        }

        return false;
    }
}
