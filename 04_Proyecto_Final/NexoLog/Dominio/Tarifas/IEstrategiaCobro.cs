namespace NexoLog.Dominio.Tarifas;

// Contrato común para las estrategias de cálculo de tarifas asociadas a despachos.
public interface IEstrategiaCobro
{
    decimal CalcularTarifa(Despacho despacho);
    string ObtenerTipoTarifa();
}
