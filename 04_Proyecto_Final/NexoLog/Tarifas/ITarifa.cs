namespace NexoLog.Tarifas;

// Contrato para el calculo de tarifas asociadas a despachos.
public interface ITarifa
{
    decimal ValorBase { get; }
    decimal CalcularTarifa(Despacho despacho);
    string ObtenerTipoTarifa();
}
