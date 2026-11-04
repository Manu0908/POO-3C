namespace NexoLog.Dominio.Modalidades;

// Modalidad Exprés: salida a ruta (RUT) <= 2 horas desde REC y entrega (ENT) <= 24 horas desde REC.
public class ServicioExpres : ModalidadServicio
{
    public ServicioExpres() : base("Exprés")
    {
    }

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
            if (string.Equals(acontecimiento.Tipo, Acontecimiento.TipoRec, StringComparison.OrdinalIgnoreCase) && eventoRec == null)
            {
                eventoRec = acontecimiento;
            }
            else if (string.Equals(acontecimiento.Tipo, Acontecimiento.TipoRut, StringComparison.OrdinalIgnoreCase) && eventoRut == null)
            {
                eventoRut = acontecimiento;
            }
            else if (string.Equals(acontecimiento.Tipo, Acontecimiento.TipoEnt, StringComparison.OrdinalIgnoreCase))
            {
                eventoEnt = acontecimiento;
            }
        }

        if (eventoRec == null || eventoRut == null || eventoEnt == null)
        {
            return false;
        }

        TimeSpan tiempoHaciaRuta = eventoRut.FechaHora - eventoRec.FechaHora;
        TimeSpan tiempoHaciaEntrega = eventoEnt.FechaHora - eventoRec.FechaHora;

        bool cumpleRuta = tiempoHaciaRuta.TotalHours >= 0 && tiempoHaciaRuta.TotalHours <= 2.0;
        bool cumpleEntrega = tiempoHaciaEntrega.TotalHours >= 0 && tiempoHaciaEntrega.TotalHours <= 24.0;

        return cumpleRuta && cumpleEntrega;
    }
}
