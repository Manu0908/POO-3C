namespace NexoLog.Dominio.Modalidades;

/// <summary>
/// Modalidad de servicio Estándar: la entrega final (ENT) debe ocurrir dentro de las
/// 72 horas posteriores a la recepción de la mercancía (REC).
/// </summary>
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
            if (acontecimiento.Tipo == TipoAcontecimiento.REC && eventoRec == null)
            {
                eventoRec = acontecimiento;
            }
            else if (acontecimiento.Tipo == TipoAcontecimiento.ENT)
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
