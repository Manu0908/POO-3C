using System.Globalization;
using System.Text;
using System.Xml.Linq;
using NexoLog.Servicios;

namespace NexoLog.Importadores;

public class ImportadorPedidosXml : IImportadorArchivo<Despacho>
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
                        RegistroInconsistencia.TipoDatoInvalido,
                        "El archivo XML de pedidos no contiene nodo raiz.",
                        DateTime.Now));
                    return despachosValidos;
                }

                string codigoComercio = (string)raiz.Attribute("codigoComercio") ?? string.Empty;
                if (string.IsNullOrWhiteSpace(codigoComercio))
                {
                    inconsistencias.Add(new RegistroInconsistencia(
                        nombreArchivo,
                        "RAIZ_SIN_COMERCIO",
                        RegistroInconsistencia.TipoDatoInvalido,
                        "El elemento raiz del XML no define el atributo 'codigoComercio'.",
                        DateTime.Now));
                    return despachosValidos;
                }

                IEnumerable<XElement> elementosSolicitud = raiz.Elements("solicitud");
                foreach (XElement elSol in elementosSolicitud)
                {
                    AgregarPedidoSiEsValido(elSol, codigoComercio, nombreArchivo, despachosValidos, inconsistencias);
                }
            }
        }
        catch (Exception ex)
        {
            inconsistencias.Add(new RegistroInconsistencia(
                nombreArchivo,
                "GENERAL",
                RegistroInconsistencia.TipoDatoInvalido,
                $"Error critico al procesar el archivo XML de pedidos: {ex.Message}",
                DateTime.Now));
        }

        return despachosValidos;
    }

    private void AgregarPedidoSiEsValido(
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
                RegistroInconsistencia.TipoDatoInvalido,
                "Se detecto un elemento <solicitud> sin atributo 'guia'.",
                DateTime.Now));
            return;
        }

        try
        {
            Despacho nuevoDespacho = ExtraerDespachoDesdeXml(elSol, numeroGuia, codigoComercio);
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

    private Despacho ExtraerDespachoDesdeXml(XElement elSol, string numeroGuia, string codigoComercio)
    {
        string destinatario = (string)elSol.Element("destinatario") ?? string.Empty;
        string ciudadDestino = (string)elSol.Element("ciudadDestino") ?? string.Empty;
        string transportadora = (string)elSol.Element("transportadora") ?? string.Empty;
        string distanciaKmTexto = (string)elSol.Element("distanciaKm") ?? "0";

        double distanciaKm;
        if (!double.TryParse(distanciaKmTexto, NumberStyles.Any, CultureInfo.InvariantCulture, out distanciaKm) || distanciaKm <= 0)
        {
            throw new NexoLogException($"Distancia en km invalida '{distanciaKmTexto}' en la guia '{numeroGuia}'.");
        }

        List<Paquete> paquetesConstruidos = ExtraerPaquetesDesdeXml(elSol, numeroGuia);

        string modalidadTexto = (string)elSol.Attribute("modalidad") ?? string.Empty;
        XElement elProg = elSol.Element("programacion");
        string fechaProgTexto = string.Empty;
        string horaIniTexto = string.Empty;
        string horaFinTexto = string.Empty;

        if (elProg != null)
        {
            fechaProgTexto = (string)elProg.Attribute("fecha") ?? string.Empty;
            horaIniTexto = (string)elProg.Attribute("horaInicio") ?? string.Empty;
            horaFinTexto = (string)elProg.Attribute("horaFin") ?? string.Empty;
        }

        ModalidadServicio modalidadConstruida = ModalidadServicio.Crear(
            modalidadTexto,
            fechaProgTexto,
            horaIniTexto,
            horaFinTexto,
            numeroGuia);

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

        return nuevoDespacho;
    }

    private List<Paquete> ExtraerPaquetesDesdeXml(XElement elSol, string numeroGuia)
    {
        XElement elPaquetes = elSol.Element("paquetes");
        List<XElement> listaNodosPaquete = elPaquetes != null ? elPaquetes.Elements("paquete").ToList() : new List<XElement>();

        if (listaNodosPaquete.Count == 0)
        {
            throw new NexoLogException($"El despacho '{numeroGuia}' no contiene paquetes asociados.");
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
                throw new NexoLogException($"Paquete sin atributo 'id' en la guia '{numeroGuia}'.");
            }

            if (codigosPaquetesVistos.Contains(idPaquete))
            {
                throw new NexoLogException($"Identificador de paquete duplicado '{idPaquete}' en la guia '{numeroGuia}'.");
            }

            double peso;
            if (!double.TryParse(pesoTexto, NumberStyles.Any, CultureInfo.InvariantCulture, out peso))
            {
                throw new NexoLogException($"Peso invalido '{pesoTexto}' para el paquete '{idPaquete}' en la guia '{numeroGuia}'.");
            }

            decimal valorDeclarado;
            if (!decimal.TryParse(valorTexto, NumberStyles.Any, CultureInfo.InvariantCulture, out valorDeclarado))
            {
                throw new NexoLogException($"Valor declarado invalido '{valorTexto}' para el paquete '{idPaquete}' en la guia '{numeroGuia}'.");
            }

            Paquete nuevoPaquete = new Paquete(idPaquete, descripcion, peso, valorDeclarado);
            codigosPaquetesVistos.Add(idPaquete);
            paquetesConstruidos.Add(nuevoPaquete);
        }

        return paquetesConstruidos;
    }
}
