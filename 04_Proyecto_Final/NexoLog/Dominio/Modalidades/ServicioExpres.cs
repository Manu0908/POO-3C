namespace NexoLog.Dominio.Modalidades;

/// <summary>
/// Modalidad de servicio Exprés: exige salida a ruta (RUT) dentro de 2 horas desde REC
/// y entrega final (ENT) dentro de 24 horas desde REC.
/// </summary>
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
            if (acontecimiento.Tipo == TipoAcontecimiento.REC && eventoRec == null)
            {
                eventoRec = acontecimiento;
            }
            else if (acontecimiento.Tipo == TipoAcontecimiento.RUT && eventoRut == null)
            {
                eventoRut = acontecimiento;
            }
            else if (acontecimiento.Tipo == TipoAcontecimiento.ENT)
            {
                eventoEnt = acontecimiento;
            }
        }

        // Si falta alguno de los acontecimientos indispensables, no cumple
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
