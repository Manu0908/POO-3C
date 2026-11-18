namespace NexoLog.Servicios;

public class ServicioEstandar : ModalidadServicio
{
    private const int _tiempoEntregaMaximoH = 72;

    public override string NombreModalidad { get { return "Estándar"; } }

    public ServicioEstandar() { }

    public override bool EvaluarCumplimiento(Despacho despacho)
    {
        if (despacho == null)
        {
            return false;
        }

        Acontecimiento eventoRec = despacho.BuscarAcontecimiento(Acontecimiento.TipoRec);
        Acontecimiento eventoEnt = despacho.BuscarAcontecimiento(Acontecimiento.TipoEnt);
        if (eventoRec == null || eventoEnt == null)
        {
            return false;
        }

        double horasTranscurridas = (eventoEnt.FechaHora - eventoRec.FechaHora).TotalHours;
        return horasTranscurridas >= 0 && horasTranscurridas <= _tiempoEntregaMaximoH;
    }
}
