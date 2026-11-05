namespace NexoLog.Servicios;

// Modalidad Estandar: la entrega (ENT) debe ocurrir dentro de las 72 horas desde la recepcion (REC).
public class ServicioEstandar : ModalidadServicio
{
    // Constructor horizontal
    public ServicioEstandar() : base("Estándar") { }

    // Evaluacion de SLA
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
