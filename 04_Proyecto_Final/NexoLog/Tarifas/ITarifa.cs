namespace NexoLog.Tarifas;

public interface ITarifa
{
    string TipoTarifa { get; }
    decimal ValorBase { get; }
    decimal CalcularTarifa(Despacho despacho);
}
