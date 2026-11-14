namespace NexoLog.Servicios;

public class ServicioEstandar : ModalidadServicio
{
    public ServicioEstandar() : base("Estándar") { }

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
            if (acontecimiento.Tipo.ToLower() == Acontecimiento.TipoRec.ToLower() && eventoRec == null)
            {
                eventoRec = acontecimiento;
            }
            else if (acontecimiento.Tipo.ToLower() == Acontecimiento.TipoEnt.ToLower())
            {
                eventoEnt = acontecimiento;
            }
        }

        if (eventoRec == null || eventoEnt == null)
        {
            return false;
        }

        double horasTranscurridas = (eventoEnt.FechaHora - eventoRec.FechaHora).TotalHours;
        return horasTranscurridas >= 0 && horasTranscurridas <= 72.0;
    }
}
