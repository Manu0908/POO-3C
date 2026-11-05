namespace NexoLog.Dominio.Tarifas;

// Contrato para el calculo de tarifas asociadas a despachos.
public interface ITarifa
{
    decimal CalcularTarifa(Despacho despacho);
    string ObtenerTipoTarifa();
}
