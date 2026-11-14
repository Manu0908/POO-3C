using System.Text.Json.Serialization;
using NexoLog.Servicios;

namespace NexoLog.DTOs;

public class PedidoJsonDto
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
    public List<PaqueteJsonDto> Paquetes { get; set; } = new List<PaqueteJsonDto>();

    public Despacho MapToDomain(string codigoEstablecimiento)
    {
        if (string.IsNullOrWhiteSpace(NumeroGuia))
        {
            throw new NexoLogException("Se detecto una solicitud sin numero de guia.");
        }

        if (Paquetes == null || Paquetes.Count == 0)
        {
            throw new NexoLogException($"El despacho '{NumeroGuia}' no contiene paquetes asociados.");
        }

        ModalidadServicio modalidadConstruida = ModalidadServicio.Crear(
            Modalidad,
            FechaProgramada,
            HoraInicio,
            HoraFin,
            NumeroGuia);

        Despacho nuevoDespacho;
        try
        {
            nuevoDespacho = new Despacho(
                NumeroGuia,
                codigoEstablecimiento,
                Destinatario,
                CiudadDestino,
                DistanciaKm,
                Transportadora);
        }
        catch (NexoLogException ex)
        {
            throw new NexoLogException($"Datos de despacho invalidos en la guia '{NumeroGuia}': {ex.Message}");
        }

        nuevoDespacho.AsignarModalidad(modalidadConstruida);

        HashSet<string> codigosPaquetesVistos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (PaqueteJsonDto pDto in Paquetes)
        {
            if (string.IsNullOrWhiteSpace(pDto.IdPaquete))
            {
                throw new NexoLogException($"Paquete sin identificador en la guia '{NumeroGuia}'.");
            }

            if (codigosPaquetesVistos.Contains(pDto.IdPaquete))
            {
                throw new NexoLogException($"Identificador de paquete duplicado '{pDto.IdPaquete}' dentro de la guia '{NumeroGuia}'.");
            }

            try
            {
                Paquete nuevoPaquete = pDto.MapToDomain();
                codigosPaquetesVistos.Add(pDto.IdPaquete);
                nuevoDespacho.AgregarPaquete(nuevoPaquete);
            }
            catch (NexoLogException ex)
            {
                throw new NexoLogException($"Paquete invalido en la guia '{NumeroGuia}': {ex.Message}");
            }
        }

        return nuevoDespacho;
    }
}
