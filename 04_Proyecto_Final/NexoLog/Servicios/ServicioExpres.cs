namespace NexoLog.Servicios;

public class ServicioExpres : ModalidadServicio
{
    private const int _tiempoInicioDistribucionMaximoH = 2;
    private const int _tiempoEntregaMaximoH = 24;

    public override string NombreModalidad { get { return "Exprés"; } }

    public ServicioExpres() { }

    public override bool EvaluarCumplimiento(Despacho despacho)
    {
        if (despacho == null)
        {
            return false;
        }

        Acontecimiento eventoRec = despacho.BuscarAcontecimiento(Acontecimiento.TipoRec);
        Acontecimiento eventoRut = despacho.BuscarAcontecimiento(Acontecimiento.TipoRut);
        Acontecimiento eventoEnt = despacho.BuscarAcontecimiento(Acontecimiento.TipoEnt);
        if (eventoRec == null || eventoRut == null || eventoEnt == null)
        {
            return false;
        }

        double horasHaciaRuta = (eventoRut.FechaHora - eventoRec.FechaHora).TotalHours;
        double horasHaciaEntrega = (eventoEnt.FechaHora - eventoRec.FechaHora).TotalHours;

        return horasHaciaRuta >= 0 && horasHaciaRuta <= _tiempoInicioDistribucionMaximoH
            && horasHaciaEntrega >= 0 && horasHaciaEntrega <= _tiempoEntregaMaximoH;
    }
}
