using System.Globalization;
using System.Text;
using System.Text.Json;
using NexoLog.Dominio;
using NexoLog.Dominio.Inconsistencias;
using NexoLog.Dominio.Modalidades;
using NexoLog.Infraestructura.Lectores.DTOs;

namespace NexoLog.Infraestructura.Lectores;

// Lector resiliente para pedidos y despachos en formato JSON.
public class LectorPedidosJson : ILectorArchivo<Despacho>
{
    public List<Despacho> Leer(string rutaArchivo, List<RegistroInconsistencia> inconsistencias)
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
                        "Se encontró un lote sin código de comercio asociado.",
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
                $"Error crítico al procesar el archivo JSON de pedidos: {ex.Message}",
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
                "Se detectó una solicitud sin número de guía.",
                DateTime.Now));
            return;
        }

        // Validación de paquetes
        if (solicitud.Paquetes == null || solicitud.Paquetes.Count == 0)
        {
            inconsistencias.Add(new RegistroInconsistencia(
                nombreArchivo,
                numeroGuia,
                RegistroInconsistencia.TipoDatoInvalido,
                $"El despacho '{numeroGuia}' no contiene paquetes asociados.",
                DateTime.Now));
            return;
        }

        List<Paquete> paquetesConstruidos = new List<Paquete>();
        HashSet<string> codigosPaquetesVistos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (PaqueteJsonDto pDto in solicitud.Paquetes)
        {
            if (string.IsNullOrWhiteSpace(pDto.IdPaquete))
            {
                inconsistencias.Add(new RegistroInconsistencia(
                    nombreArchivo,
                    numeroGuia,
                    RegistroInconsistencia.TipoDatoInvalido,
                    $"Paquete sin identificador en la guía '{numeroGuia}'.",
                    DateTime.Now));
                return;
            }

            if (codigosPaquetesVistos.Contains(pDto.IdPaquete))
            {
                inconsistencias.Add(new RegistroInconsistencia(
                    nombreArchivo,
                    numeroGuia,
                    RegistroInconsistencia.TipoDatoInvalido,
                    $"Identificador de paquete duplicado '{pDto.IdPaquete}' dentro de la guía '{numeroGuia}'.",
                    DateTime.Now));
                return;
            }

            try
            {
                Paquete nuevoPaquete = new Paquete(
                    pDto.IdPaquete,
                    pDto.Descripcion,
                    pDto.Peso,
                    pDto.ValorDeclarado);

                codigosPaquetesVistos.Add(pDto.IdPaquete);
                paquetesConstruidos.Add(nuevoPaquete);
            }
            catch (NexoLogException ex)
            {
                inconsistencias.Add(new RegistroInconsistencia(
                    nombreArchivo,
                    numeroGuia,
                    RegistroInconsistencia.TipoDatoInvalido,
                    $"Paquete inválido en la guía '{numeroGuia}': {ex.Message}",
                    DateTime.Now));
                return;
            }
        }

        // Construcción de la modalidad
        ModalidadServicio modalidadConstruida;
        string modalidadTexto = (solicitud.Modalidad ?? string.Empty).Trim().ToUpperInvariant();

        try
        {
            if (modalidadTexto == "ESTANDAR" || modalidadTexto == "ESTÁNDAR")
            {
                modalidadConstruida = new ServicioEstandar();
            }
            else if (modalidadTexto == "EXPRES" || modalidadTexto == "EXPRÉS")
            {
                modalidadConstruida = new ServicioExpres();
            }
            else if (modalidadTexto == "PROGRAMADO")
            {
                if (!DateTime.TryParse(solicitud.FechaProgramada, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaProg))
                {
                    inconsistencias.Add(new RegistroInconsistencia(
                        nombreArchivo,
                        numeroGuia,
                        RegistroInconsistencia.TipoDatoInvalido,
                        $"Fecha programada inválida '{solicitud.FechaProgramada}' en la guía '{numeroGuia}'.",
                        DateTime.Now));
                    return;
                }

                if (!TimeSpan.TryParse(solicitud.HoraInicio, CultureInfo.InvariantCulture, out TimeSpan horaIni))
                {
                    inconsistencias.Add(new RegistroInconsistencia(
                        nombreArchivo,
                        numeroGuia,
                        RegistroInconsistencia.TipoDatoInvalido,
                        $"Hora inicial programada inválida '{solicitud.HoraInicio}' en la guía '{numeroGuia}'.",
                        DateTime.Now));
                    return;
                }

                if (!TimeSpan.TryParse(solicitud.HoraFin, CultureInfo.InvariantCulture, out TimeSpan horaFin))
                {
                    inconsistencias.Add(new RegistroInconsistencia(
                        nombreArchivo,
                        numeroGuia,
                        RegistroInconsistencia.TipoDatoInvalido,
                        $"Hora final programada inválida '{solicitud.HoraFin}' en la guía '{numeroGuia}'.",
                        DateTime.Now));
                    return;
                }

                modalidadConstruida = new ServicioProgramado(fechaProg, horaIni, horaFin);
            }
            else
            {
                inconsistencias.Add(new RegistroInconsistencia(
                    nombreArchivo,
                    numeroGuia,
                    RegistroInconsistencia.TipoDatoInvalido,
                    $"Modalidad de servicio desconocida '{solicitud.Modalidad}' en la guía '{numeroGuia}'.",
                    DateTime.Now));
                return;
            }
        }
        catch (NexoLogException ex)
        {
            inconsistencias.Add(new RegistroInconsistencia(
                nombreArchivo,
                numeroGuia,
                RegistroInconsistencia.TipoDatoInvalido,
                $"Modalidad inválida en la guía '{numeroGuia}': {ex.Message}",
                DateTime.Now));
            return;
        }

        // Construcción del despacho
        try
        {
            Despacho nuevoDespacho = new Despacho(
                numeroGuia,
                codigoEstablecimiento,
                solicitud.Destinatario,
                solicitud.CiudadDestino,
                solicitud.DistanciaKm,
                solicitud.Transportadora);

            nuevoDespacho.AsignarModalidad(modalidadConstruida);

            foreach (Paquete p in paquetesConstruidos)
            {
                nuevoDespacho.AgregarPaquete(p);
            }

            listaDestino.Add(nuevoDespacho);
        }
        catch (NexoLogException ex)
        {
            inconsistencias.Add(new RegistroInconsistencia(
                nombreArchivo,
                numeroGuia,
                RegistroInconsistencia.TipoDatoInvalido,
                $"Datos de despacho inválidos en la guía '{numeroGuia}': {ex.Message}",
                DateTime.Now));
        }
    }
}
