namespace NexoLog.Servicios;

public class ServicioExpres : ModalidadServicio
{
    public ServicioExpres() : base("Exprés") { }

    public override bool EvaluarCumplimiento(Despacho despacho)
    {
        if (despacho == null)
        {
            return false;
        }

        Acontecimiento eventoRec = null;
        Acontecimiento eventoRut = null;
        Acontecimiento eventoEnt = null;

        foreach (Acontecimiento acontecimiento in despacho.Historial)
        {
            if (acontecimiento.Tipo.ToLower() == Acontecimiento.TipoRec.ToLower() && eventoRec == null)
            {
                eventoRec = acontecimiento;
            }
            else if (acontecimiento.Tipo.ToLower() == Acontecimiento.TipoRut.ToLower() && eventoRut == null)
            {
                eventoRut = acontecimiento;
            }
            else if (acontecimiento.Tipo.ToLower() == Acontecimiento.TipoEnt.ToLower())
            {
                eventoEnt = acontecimiento;
            }
        }

        if (eventoRec == null || eventoRut == null || eventoEnt == null)
        {
            return false;
        }

        double horasHaciaRuta = (eventoRut.FechaHora - eventoRec.FechaHora).TotalHours;
        double horasHaciaEntrega = (eventoEnt.FechaHora - eventoRec.FechaHora).TotalHours;

        bool cumpleRuta = horasHaciaRuta >= 0 && horasHaciaRuta <= 2.0;
        bool cumpleEntrega = horasHaciaEntrega >= 0 && horasHaciaEntrega <= 24.0;

        return cumpleRuta && cumpleEntrega;
    }
}
