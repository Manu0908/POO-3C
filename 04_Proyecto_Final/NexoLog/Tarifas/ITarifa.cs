namespace NexoLog.Tarifas;

public interface ITarifa
{
    string TipoTarifa { get; }
    double ValorBase { get; }
    double CalcularTarifa(Despacho despacho);
}
