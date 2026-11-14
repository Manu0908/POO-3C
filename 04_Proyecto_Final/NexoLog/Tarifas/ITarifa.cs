namespace NexoLog.Tarifas;

public interface ITarifa
{
    decimal ValorBase { get; }
    decimal CalcularTarifa(Despacho despacho);
    string ObtenerTipoTarifa();
}
