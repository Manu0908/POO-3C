namespace NexoLog.Servicios;

public class ServicioProgramado : ModalidadServicio
{
    private DateTime _inicio;
    private DateTime _fin;

    public override string NombreModalidad => "Programado";

    private ServicioProgramado() { }

    public ServicioProgramado(DateTime inicio, DateTime fin)
    {
        if (fin <= inicio)
        {
            throw new NexoLogException("La fecha y hora final de la programacion debe ser estrictamente posterior a la inicial.");
        }
        _inicio = inicio;
        _fin = fin;
    }

    public DateTime Inicio => _inicio;
    public DateTime Fin => _fin;

    public override bool Cumple(Despacho despacho)
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

        if (eventoRut != null && eventoRut.FechaHora.Date < _inicio.Date)
        {
            return false;
        }

        if (eventoEnt.FechaHora >= _inicio && eventoEnt.FechaHora <= _fin)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
