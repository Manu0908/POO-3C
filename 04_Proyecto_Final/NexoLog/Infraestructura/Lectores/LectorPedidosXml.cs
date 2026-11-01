using System.Globalization;
using System.Text;
using System.Xml.Linq;
using NexoLog.Dominio;
using NexoLog.Dominio.Inconsistencias;
using NexoLog.Dominio.Modalidades;

namespace NexoLog.Infraestructura.Lectores;

/// <summary>
/// Lector resiliente para solicitudes de pedidos en formato XML estructurado.
/// </summary>
public class LectorPedidosXml : ILectorArchivo<Despacho>
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
                TipoInconsistencia.ReferenciaInexistente,
                $"El archivo XML de pedidos '{rutaArchivo}' no existe.",
                DateTime.Now));
            return despachosValidos;
        }

        try
        {
            using (FileStream stream = new FileStream(rutaArchivo, FileMode.Open, FileAccess.Read))
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                XDocument documento = XDocument.Load(reader);
                XElement raiz = documento.Root;

                if (raiz == null)
                {
                    inconsistencias.Add(new RegistroInconsistencia(
                        nombreArchivo,
                        "GENERAL",
                        TipoInconsistencia.DatoInvalido,
                        "El archivo XML de pedidos no contiene nodo raíz.",
                        DateTime.Now));
                    return despachosValidos;
                }

                string codigoComercio = (string)raiz.Attribute("codigoComercio") ?? string.Empty;
                if (string.IsNullOrWhiteSpace(codigoComercio))
                {
                    inconsistencias.Add(new RegistroInconsistencia(
                        nombreArchivo,
                        "RAIZ_SIN_COMERCIO",
                        TipoInconsistencia.DatoInvalido,
                        "El elemento raíz del XML no define el atributo 'codigoComercio'.",
                        DateTime.Now));
                    return despachosValidos;
                }

                IEnumerable<XElement> elementosSolicitud = raiz.Elements("solicitud");
                foreach (XElement elSol in elementosSolicitud)
                {
                    ProcesarElementoSolicitud(elSol, codigoComercio, nombreArchivo, despachosValidos, inconsistencias);
                }
            }
        }
        catch (Exception ex)
        {
            inconsistencias.Add(new RegistroInconsistencia(
                nombreArchivo,
                "GENERAL",
                TipoInconsistencia.DatoInvalido,
                $"Error crítico al procesar el archivo XML de pedidos: {ex.Message}",
                DateTime.Now));
        }

        return despachosValidos;
    }

    private void ProcesarElementoSolicitud(
        XElement elSol,
        string codigoComercio,
        string nombreArchivo,
        List<Despacho> listaDestino,
        List<RegistroInconsistencia> inconsistencias)
    {
        string numeroGuia = (string)elSol.Attribute("guia") ?? string.Empty;
        if (string.IsNullOrWhiteSpace(numeroGuia))
        {
            inconsistencias.Add(new RegistroInconsistencia(
                nombreArchivo,
                "SOLICITUD_SIN_GUIA",
                TipoInconsistencia.DatoInvalido,
                "Se detectó un elemento <solicitud> sin atributo 'guia'.",
                DateTime.Now));
            return;
        }

        string destinatario = (string)elSol.Element("destinatario") ?? string.Empty;
        string ciudadDestino = (string)elSol.Element("ciudadDestino") ?? string.Empty;
        string transportadora = (string)elSol.Element("transportadora") ?? string.Empty;
        string distanciaKmTexto = (string)elSol.Element("distanciaKm") ?? "0";

        if (!double.TryParse(distanciaKmTexto, NumberStyles.Any, CultureInfo.InvariantCulture, out double distanciaKm) || distanciaKm <= 0)
        {
            inconsistencias.Add(new RegistroInconsistencia(
                nombreArchivo,
                numeroGuia,
                TipoInconsistencia.DatoInvalido,
                $"Distancia en km inválida '{distanciaKmTexto}' en la guía '{numeroGuia}'.",
                DateTime.Now));
            return;
        }

        // Validación de paquetes
        XElement elPaquetes = elSol.Element("paquetes");
        List<XElement> listaNodosPaquete = elPaquetes != null ? elPaquetes.Elements("paquete").ToList() : new List<XElement>();

        if (listaNodosPaquete.Count == 0)
        {
            inconsistencias.Add(new RegistroInconsistencia(
                nombreArchivo,
                numeroGuia,
                TipoInconsistencia.DatoInvalido,
                $"El despacho '{numeroGuia}' no contiene paquetes asociados.",
                DateTime.Now));
            return;
        }

        List<Paquete> paquetesConstruidos = new List<Paquete>();
        HashSet<string> codigosPaquetesVistos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (XElement elPaq in listaNodosPaquete)
        {
            string idPaquete = (string)elPaq.Attribute("id") ?? string.Empty;
            string descripcion = (string)elPaq.Element("descripcion") ?? string.Empty;
            string pesoTexto = (string)elPaq.Element("peso") ?? "0";
            string valorTexto = (string)elPaq.Element("valorDeclarado") ?? "0";

            if (string.IsNullOrWhiteSpace(idPaquete))
            {
                inconsistencias.Add(new RegistroInconsistencia(
                    nombreArchivo,
                    numeroGuia,
                    TipoInconsistencia.DatoInvalido,
                    $"Paquete sin atributo 'id' en la guía '{numeroGuia}'.",
                    DateTime.Now));
                return;
            }

            if (codigosPaquetesVistos.Contains(idPaquete))
            {
                inconsistencias.Add(new RegistroInconsistencia(
                    nombreArchivo,
                    numeroGuia,
                    TipoInconsistencia.DatoInvalido,
                    $"Identificador de paquete duplicado '{idPaquete}' en la guía '{numeroGuia}'.",
                    DateTime.Now));
                return;
            }

            if (!double.TryParse(pesoTexto, NumberStyles.Any, CultureInfo.InvariantCulture, out double peso))
            {
                inconsistencias.Add(new RegistroInconsistencia(
                    nombreArchivo,
                    numeroGuia,
                    TipoInconsistencia.DatoInvalido,
                    $"Peso inválido '{pesoTexto}' para el paquete '{idPaquete}' en la guía '{numeroGuia}'.",
                    DateTime.Now));
                return;
            }

            if (!decimal.TryParse(valorTexto, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valorDeclarado))
            {
                inconsistencias.Add(new RegistroInconsistencia(
                    nombreArchivo,
                    numeroGuia,
                    TipoInconsistencia.DatoInvalido,
                    $"Valor declarado inválido '{valorTexto}' para el paquete '{idPaquete}' en la guía '{numeroGuia}'.",
                    DateTime.Now));
                return;
            }

            try
            {
                Paquete nuevoPaquete = new Paquete(idPaquete, descripcion, peso, valorDeclarado);
                codigosPaquetesVistos.Add(idPaquete);
                paquetesConstruidos.Add(nuevoPaquete);
            }
            catch (NexoLogException ex)
            {
                inconsistencias.Add(new RegistroInconsistencia(
                    nombreArchivo,
                    numeroGuia,
                    TipoInconsistencia.DatoInvalido,
                    $"Paquete inválido en la guía '{numeroGuia}': {ex.Message}",
                    DateTime.Now));
                return;
            }
        }

        // Determinación de la modalidad de servicio
        string modalidadTexto = ((string)elSol.Attribute("modalidad") ?? string.Empty).Trim().ToUpperInvariant();
        ModalidadServicio modalidadConstruida;

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
                XElement elProg = elSol.Element("programacion");
                if (elProg == null)
                {
                    inconsistencias.Add(new RegistroInconsistencia(
                        nombreArchivo,
                        numeroGuia,
                        TipoInconsistencia.DatoInvalido,
                        $"La guía '{numeroGuia}' con modalidad PROGRAMADO carece de etiqueta <programacion>.",
                        DateTime.Now));
                    return;
                }

                string fechaProgTexto = (string)elProg.Attribute("fecha") ?? string.Empty;
                string horaIniTexto = (string)elProg.Attribute("horaInicio") ?? string.Empty;
                string horaFinTexto = (string)elProg.Attribute("horaFin") ?? string.Empty;

                if (!DateTime.TryParse(fechaProgTexto, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaProg))
                {
                    inconsistencias.Add(new RegistroInconsistencia(
                        nombreArchivo,
                        numeroGuia,
                        TipoInconsistencia.DatoInvalido,
                        $"Fecha programada XML inválida '{fechaProgTexto}' en la guía '{numeroGuia}'.",
                        DateTime.Now));
                    return;
                }

                if (!TimeSpan.TryParse(horaIniTexto, CultureInfo.InvariantCulture, out TimeSpan horaIni))
                {
                    inconsistencias.Add(new RegistroInconsistencia(
                        nombreArchivo,
                        numeroGuia,
                        TipoInconsistencia.DatoInvalido,
                        $"Hora inicial XML inválida '{horaIniTexto}' en la guía '{numeroGuia}'.",
                        DateTime.Now));
                    return;
                }

                if (!TimeSpan.TryParse(horaFinTexto, CultureInfo.InvariantCulture, out TimeSpan horaFin))
                {
                    inconsistencias.Add(new RegistroInconsistencia(
                        nombreArchivo,
                        numeroGuia,
                        TipoInconsistencia.DatoInvalido,
                        $"Hora final XML inválida '{horaFinTexto}' en la guía '{numeroGuia}'.",
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
                    TipoInconsistencia.DatoInvalido,
                    $"Modalidad XML desconocida '{modalidadTexto}' en la guía '{numeroGuia}'.",
                    DateTime.Now));
                return;
            }
        }
        catch (NexoLogException ex)
        {
            inconsistencias.Add(new RegistroInconsistencia(
                nombreArchivo,
                numeroGuia,
                TipoInconsistencia.DatoInvalido,
                $"Modalidad inválida en la guía '{numeroGuia}': {ex.Message}",
                DateTime.Now));
            return;
        }

        // Construcción y registro del Despacho
        try
        {
            Despacho nuevoDespacho = new Despacho(
                numeroGuia,
                codigoComercio,
                destinatario,
                ciudadDestino,
                distanciaKm,
                transportadora);

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
                TipoInconsistencia.DatoInvalido,
                $"Datos de despacho inválidos en la guía '{numeroGuia}': {ex.Message}",
                DateTime.Now));
        }
    }
}
