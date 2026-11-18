using System.Text.Json.Serialization;
using NexoLog.Servicios;

namespace NexoLog.DTOs;

public class ProgramacionDto
{
    [JsonPropertyName("inicio")]
    public DateTime Inicio { get; set; }

    [JsonPropertyName("fin")]
    public DateTime Fin { get; set; }
}

public class PedidoDto
{
    [JsonPropertyName("guia")]
    public string NumeroGuia { get; set; } = string.Empty;

    [JsonPropertyName("servicio")]
    public string Modalidad { get; set; } = string.Empty;

    [JsonPropertyName("transportadora")]
    public string Transportadora { get; set; } = string.Empty;

    [JsonPropertyName("destinatario")]
    public string Destinatario { get; set; } = string.Empty;

    [JsonPropertyName("ciudad")]
    public string CiudadDestino { get; set; } = string.Empty;

    [JsonPropertyName("distanciaKm")]
    public double DistanciaKm { get; set; }

    [JsonPropertyName("programacion")]
    public ProgramacionDto Programacion { get; set; }

    [JsonPropertyName("paquetes")]
    public List<PaqueteDto> Paquetes { get; set; } = new List<PaqueteDto>();

    public Despacho MapToDomain(string codigoEstablecimiento, string nombreEstablecimiento)
    {
        if (Paquetes == null || Paquetes.Count == 0)
        {
            throw new NexoLogException($"El despacho '{NumeroGuia}' no contiene paquetes asociados.");
        }

        DateTime? inicio = Programacion != null ? Programacion.Inicio : null;
        DateTime? fin = Programacion != null ? Programacion.Fin : null;
        ModalidadServicio modalidad = ModalidadServicio.Crear(Modalidad, inicio, fin, NumeroGuia);

        Despacho despacho = new Despacho(NumeroGuia, codigoEstablecimiento, Destinatario, CiudadDestino, DistanciaKm, Transportadora);
        despacho.AsignarModalidad(modalidad);
        despacho.AsignarNombreEstablecimiento(nombreEstablecimiento);

        foreach (PaqueteDto p in Paquetes)
        {
            despacho.AgregarPaquete(p.MapToDomain());
        }

        return despacho;
    }
}
