namespace NexoLog.Dominio.Modalidades;

// Modalidad Estándar: la entrega final (ENT) debe ocurrir dentro de las 72 horas desde REC.
public class ServicioEstandar : ModalidadServicio
{
    public ServicioEstandar() : base("Estándar")
    {
    }

    public override bool EvaluarCumplimiento(Despacho despacho)
    {
        if (despacho == null)
        {
            return false;
        }

        Acontecimiento eventoRec = null;
        Acontecimiento eventoEnt = null;

        foreach (Acontecimiento acontecimiento in despacho.Historial)
        {
            if (string.Equals(acontecimiento.Tipo, Acontecimiento.TipoRec, StringComparison.OrdinalIgnoreCase) && eventoRec == null)
            {
                eventoRec = acontecimiento;
            }
            else if (string.Equals(acontecimiento.Tipo, Acontecimiento.TipoEnt, StringComparison.OrdinalIgnoreCase))
            {
                eventoEnt = acontecimiento;
            }
        }

        if (eventoRec == null || eventoEnt == null)
        {
            return false;
        }

        TimeSpan tiempoTranscurrido = eventoEnt.FechaHora - eventoRec.FechaHora;
        return tiempoTranscurrido.TotalHours >= 0 && tiempoTranscurrido.TotalHours <= 72.0;
    }
}
