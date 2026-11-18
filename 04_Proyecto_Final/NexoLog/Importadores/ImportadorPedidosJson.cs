using System.Text;
using System.Text.Json;
using NexoLog.DTOs;

namespace NexoLog.Importadores;

public class ImportadorPedidosJson : IImportadorArchivo<Despacho>
{
    public List<Despacho> Importar(string rutaArchivo, List<RegistroInconsistencia> inconsistencias)
    {
        List<Despacho> despachosValidos = new List<Despacho>();
        string nombreArchivo = Path.GetFileName(rutaArchivo);

        try
        {
            string contenidoJson = File.ReadAllText(rutaArchivo, Encoding.UTF8);
            JsonSerializerOptions opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<LoteEstablecimientoDto> lotes = JsonSerializer.Deserialize<List<LoteEstablecimientoDto>>(contenidoJson, opciones) ?? new List<LoteEstablecimientoDto>();

            foreach (LoteEstablecimientoDto lote in lotes)
            {
                foreach (PedidoDto solicitud in lote.Solicitudes)
                {
                    try
                    {
                        despachosValidos.Add(solicitud.MapToDomain(lote.CodigoEstablecimiento));
                    }
                    catch (NexoLogException ex)
                    {
                        string refGuia = string.IsNullOrWhiteSpace(solicitud.NumeroGuia) ? "SOLICITUD_SIN_GUIA" : solicitud.NumeroGuia;
                        inconsistencias.Add(new RegistroInconsistencia(nombreArchivo, refGuia, RegistroInconsistencia.TipoDatoInvalido, ex.Message, DateTime.Now));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            inconsistencias.Add(new RegistroInconsistencia(nombreArchivo, "GENERAL", RegistroInconsistencia.TipoDatoInvalido, ex.Message, DateTime.Now));
        }

        return despachosValidos;
    }
}
