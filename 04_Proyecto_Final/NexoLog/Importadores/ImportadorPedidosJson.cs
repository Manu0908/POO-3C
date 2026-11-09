using System.Globalization;
using System.Text;
using System.Text.Json;
using NexoLog.DTOs;
using NexoLog.Servicios;

namespace NexoLog.Importadores;

// Importador resiliente para pedidos y despachos en formato JSON.
public class ImportadorPedidosJson : IImportadorArchivo<Despacho>
{
    public List<Despacho> Importar(string rutaArchivo, List<RegistroInconsistencia> inconsistencias)
    {
        List<Despacho> despachosValidos = new List<Despacho>();
        string nombreArchivo = Path.GetFileName(rutaArchivo);

        if (!File.Exists(rutaArchivo))
        {
            inconsistencias.Add(new RegistroInconsistencia(
                nombreArchivo,
                "GENERAL",
                RegistroInconsistencia.TipoReferenciaInexistente,
                $"El archivo de pedidos '{rutaArchivo}' no existe.",
                DateTime.Now));
            return despachosValidos;
        }

        try
        {
            string contenidoJson = File.ReadAllText(rutaArchivo, Encoding.UTF8);
            JsonSerializerOptions opciones = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            List<LoteEstablecimientoDto> lotes = JsonSerializer.Deserialize<List<LoteEstablecimientoDto>>(contenidoJson, opciones);

            if (lotes == null || lotes.Count == 0)
            {
                inconsistencias.Add(new RegistroInconsistencia(
                    nombreArchivo,
                    "GENERAL",
                    RegistroInconsistencia.TipoDatoInvalido,
                    "El archivo JSON de pedidos no contiene lotes ni comercios remitentes.",
                    DateTime.Now));
                return despachosValidos;
            }

            foreach (LoteEstablecimientoDto lote in lotes)
            {
                string codigoEstablecimiento = lote.CodigoEstablecimiento;
                if (string.IsNullOrWhiteSpace(codigoEstablecimiento))
                {
                    inconsistencias.Add(new RegistroInconsistencia(
                        nombreArchivo,
                        "LOTE_SIN_COMERCIO",
                        RegistroInconsistencia.TipoDatoInvalido,
                        "Se encontro un lote sin codigo de comercio asociado.",
                        DateTime.Now));
                    continue;
                }

                foreach (PedidoJsonDto solicitud in lote.Solicitudes)
                {
                    ProcesarSolicitud(solicitud, codigoEstablecimiento, nombreArchivo, despachosValidos, inconsistencias);
                }
            }
        }
        catch (Exception ex)
        {
            inconsistencias.Add(new RegistroInconsistencia(
                nombreArchivo,
                "GENERAL",
                RegistroInconsistencia.TipoDatoInvalido,
                $"Error critico al procesar el archivo JSON de pedidos: {ex.Message}",
                DateTime.Now));
        }

        return despachosValidos;
    }

    private void ProcesarSolicitud(
        PedidoJsonDto solicitud,
        string codigoEstablecimiento,
        string nombreArchivo,
        List<Despacho> listaDestino,
        List<RegistroInconsistencia> inconsistencias)
    {
        string numeroGuia = solicitud.NumeroGuia;

        if (string.IsNullOrWhiteSpace(numeroGuia))
        {
            inconsistencias.Add(new RegistroInconsistencia(
                nombreArchivo,
                "SOLICITUD_SIN_GUIA",
                RegistroInconsistencia.TipoDatoInvalido,
                "Se detecto una solicitud sin numero de guia.",
                DateTime.Now));
            return;
        }

        try
        {
            Despacho nuevoDespacho = solicitud.MapToDomain(codigoEstablecimiento);
            listaDestino.Add(nuevoDespacho);
        }
        catch (NexoLogException ex)
        {
            inconsistencias.Add(new RegistroInconsistencia(
                nombreArchivo,
                numeroGuia,
                RegistroInconsistencia.TipoDatoInvalido,
                ex.Message,
                DateTime.Now));
        }
    }
}

