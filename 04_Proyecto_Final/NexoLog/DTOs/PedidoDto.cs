using System.Text.Json.Serialization;
using NexoLog.Servicios;

namespace NexoLog.DTOs;

public class PedidoDto
{
    [JsonPropertyName("numeroGuia")]
    public string NumeroGuia { get; set; } = string.Empty;

    [JsonPropertyName("destinatario")]
    public string Destinatario { get; set; } = string.Empty;

    [JsonPropertyName("ciudadDestino")]
    public string CiudadDestino { get; set; } = string.Empty;

    [JsonPropertyName("distanciaKm")]
    public double DistanciaKm { get; set; }

    [JsonPropertyName("transportadora")]
    public string Transportadora { get; set; } = string.Empty;

    [JsonPropertyName("modalidad")]
    public string Modalidad { get; set; } = string.Empty;

    [JsonPropertyName("fechaProgramada")]
    public string FechaProgramada { get; set; } = string.Empty;

    [JsonPropertyName("horaInicio")]
    public string HoraInicio { get; set; } = string.Empty;

    [JsonPropertyName("horaFin")]
    public string HoraFin { get; set; } = string.Empty;

    [JsonPropertyName("paquetes")]
    public List<PaqueteDto> Paquetes { get; set; } = new List<PaqueteDto>();

    public Despacho MapToDomain(string codigoEstablecimiento)
    {
        if (Paquetes == null || Paquetes.Count == 0)
        {
            throw new NexoLogException($"El despacho '{NumeroGuia}' no contiene paquetes asociados.");
        }

        ModalidadServicio modalidad = ModalidadServicio.Crear(Modalidad, FechaProgramada, HoraInicio, HoraFin, NumeroGuia);
        Despacho despacho = new Despacho(NumeroGuia, codigoEstablecimiento, Destinatario, CiudadDestino, DistanciaKm, Transportadora);
        despacho.AsignarModalidad(modalidad);

        foreach (PaqueteDto p in Paquetes)
        {
            despacho.AgregarPaquete(p.MapToDomain());
        }

        return despacho;
    }
}
