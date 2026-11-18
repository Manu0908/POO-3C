using System.Text;
using System.Text.Json;
using NexoLog.DTOs;

namespace NexoLog.Importadores;

public class ImportadorPedidosJson : IImportadorArchivo<Despacho>
{
    public List<Despacho> Importar(string rutaArchivo, List<RegistroInconsistencia> inconsistencias)
    {
        var despachosValidos = new List<Despacho>();

        try
        {
            string contenidoJson = File.ReadAllText(rutaArchivo, Encoding.UTF8);
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var raiz = JsonSerializer.Deserialize<RaizPedidosDto>(contenidoJson, opciones);
            var lotes = new List<LoteEstablecimientoDto>();
            if (raiz != null)
            {
                lotes = raiz.Comercios;
            }

            foreach (var lote in lotes)
            {
                foreach (var solicitud in lote.Solicitudes)
                {
                    try
                    {
                        despachosValidos.Add(Despacho.MapToDomain(solicitud, lote.CodigoEstablecimiento, lote.NombreComercio));
                    }
                    catch (NexoLogException ex)
                    {
                        string refGuia = "SOLICITUD_SIN_GUIA";
                        if (!string.IsNullOrWhiteSpace(solicitud.NumeroGuia))
                        {
                            refGuia = solicitud.NumeroGuia;
                        }
                        inconsistencias.Add(new RegistroInconsistencia(rutaArchivo, refGuia, RegistroInconsistencia.TipoDatoInvalido, ex.Message, DateTime.Now));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            inconsistencias.Add(new RegistroInconsistencia(rutaArchivo, "GENERAL", RegistroInconsistencia.TipoDatoInvalido, ex.Message, DateTime.Now));
        }

        return despachosValidos;
    }
}
