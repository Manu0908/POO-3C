using System.Globalization;
using System.Text;
using System.Xml.Linq;
using NexoLog.DTOs;

namespace NexoLog.Importadores;

public class ImportadorPedidosXml : IImportadorArchivo<Despacho>
{
    public List<Despacho> Importar(string rutaArchivo, List<RegistroInconsistencia> inconsistencias)
    {
        var despachosValidos = new List<Despacho>();

        try
        {
            string contenidoXml = File.ReadAllText(rutaArchivo, Encoding.UTF8);
            var documento = XDocument.Parse(contenidoXml);
            var raiz = documento.Root;

            string codigoComercio = "";
            if (raiz.Attribute("comercio") != null)
            {
                codigoComercio = raiz.Attribute("comercio").Value;
            }
            string nombreComercio = ObtenerTexto(raiz, "Nombre", "");

            foreach (var elSol in raiz.Elements("Solicitud"))
            {
                string numeroGuia = ObtenerTexto(elSol, "NumeroGuia", "SOLICITUD_SIN_GUIA");
                try
                {
                    var dto = LeerPedidoDto(elSol);
                    despachosValidos.Add(Despacho.MapToDomain(dto, codigoComercio, nombreComercio));
                }
                catch (Exception ex)
                {
                    inconsistencias.Add(new RegistroInconsistencia(rutaArchivo, numeroGuia, RegistroInconsistencia.TipoDatoInvalido, ex.Message, DateTime.Now));
                }
            }
        }
        catch (Exception ex)
        {
            inconsistencias.Add(new RegistroInconsistencia(rutaArchivo, "GENERAL", RegistroInconsistencia.TipoDatoInvalido, ex.Message, DateTime.Now));
        }

        return despachosValidos;
    }

    private PedidoDto LeerPedidoDto(XElement elSol)
    {
        var elProg = elSol.Element("Programacion");
        ProgramacionDto progDto = null;
        if (elProg != null)
        {
            progDto = new ProgramacionDto
            {
                Inicio = DateTime.Parse(ObtenerTexto(elProg, "Inicio", ""), CultureInfo.InvariantCulture),
                Fin = DateTime.Parse(ObtenerTexto(elProg, "Fin", ""), CultureInfo.InvariantCulture)
            };
        }

        var dto = new PedidoDto
        {
            NumeroGuia = ObtenerTexto(elSol, "NumeroGuia", ""),
            Modalidad = ObtenerTexto(elSol, "Tipo", ""),
            Transportadora = ObtenerTexto(elSol, "Operador", ""),
            Destinatario = ObtenerTexto(elSol, "Cliente", ""),
            CiudadDestino = ObtenerTexto(elSol, "Destino", ""),
            DistanciaKm = double.Parse(ObtenerTexto(elSol, "Distancia", "0"), CultureInfo.InvariantCulture),
            Programacion = progDto
        };

        var elBultos = elSol.Element("Bultos");
        if (elBultos != null)
        {
            foreach (var elBulto in elBultos.Elements("Bulto"))
            {
                dto.Paquetes.Add(new PaqueteDto
                {
                    IdPaquete = ObtenerTexto(elBulto, "Referencia", ""),
                    Descripcion = ObtenerTexto(elBulto, "Descripcion", ""),
                    Peso = double.Parse(ObtenerTexto(elBulto, "Peso", "0"), CultureInfo.InvariantCulture),
                    ValorDeclarado = double.Parse(ObtenerTexto(elBulto, "Valor", "0"), CultureInfo.InvariantCulture)
                });
            }
        }

        return dto;
    }

    private string ObtenerTexto(XElement elemento, string nombreEtiqueta, string valorDefecto)
    {
        var hijo = elemento.Element(nombreEtiqueta);
        if (hijo == null)
        {
            return valorDefecto;
        }
        return hijo.Value;
    }
}
