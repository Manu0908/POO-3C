using System.Text.Json.Serialization;

namespace NexoLog.DTOs;

// DTO para deserializar acontecimientos de seguimiento desde JSON.
public class AcontecimientoJsonDto
{
    [JsonPropertyName("idAcontecimiento")]
    public string IdAcontecimiento { get; set; } = string.Empty;

    [JsonPropertyName("numeroGuia")]
    public string NumeroGuia { get; set; } = string.Empty;

    [JsonPropertyName("tipo")]
    public string Tipo { get; set; } = string.Empty;

    [JsonPropertyName("fechaHora")]
    public DateTime FechaHora { get; set; }

    [JsonPropertyName("observacion")]
    public string Observacion { get; set; } = string.Empty;

    public Acontecimiento MapToDomain()
    {
        string idEvt = IdAcontecimiento;
        string guia = NumeroGuia;

        if (string.IsNullOrWhiteSpace(idEvt) || string.IsNullOrWhiteSpace(guia))
        {
            throw new NexoLogException("Se detecto un acontecimiento sin identificador o sin numero de guia.");
        }

        string tipoLimpio = (Tipo ?? string.Empty).Trim().ToUpperInvariant();
        if (tipoLimpio != Acontecimiento.TipoRec &&
            tipoLimpio != Acontecimiento.TipoRut &&
            tipoLimpio != Acontecimiento.TipoEnt &&
            tipoLimpio != Acontecimiento.TipoDev)
        {
            throw new NexoLogException($"Tipo de acontecimiento desconocido '{Tipo}' en el evento '{idEvt}'.");
        }

        return new Acontecimiento(idEvt, guia, tipoLimpio, FechaHora);
    }
}
