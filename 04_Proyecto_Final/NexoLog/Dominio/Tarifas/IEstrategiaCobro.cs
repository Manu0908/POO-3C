namespace NexoLog.Dominio.Tarifas;

/// <summary>
/// Contrato común para las estrategias de cálculo de tarifas asociadas a despachos.
/// </summary>
public interface IEstrategiaCobro
{
    /// <summary>
    /// Calcula la tarifa correspondiente a un despacho entregado.
    /// </summary>
    decimal CalcularTarifa(Despacho despacho);

    /// <summary>
    /// Retorna la denominación descriptiva de la estrategia tarifaria aplicada.
    /// </summary>
    string ObtenerTipoTarifa();
}
