using System.Globalization;
using System.Text;
using System.Xml.Linq;
using NexoLog.DTOs;

namespace NexoLog.Importadores;

public class ImportadorPedidosXml : IImportadorArchivo<Despacho>
{
    public List<Despacho> Importar(string rutaArchivo, List<RegistroInconsistencia> inconsistencias)
    {
        List<Despacho> despachosValidos = new List<Despacho>();
        string nombreArchivo = Path.GetFileName(rutaArchivo);

        try
        {
            string contenidoXml = File.ReadAllText(rutaArchivo, Encoding.UTF8);
            XDocument documento = XDocument.Parse(contenidoXml);
            XElement raiz = documento.Root;
            string codigoComercio = (string)raiz.Attribute("comercio") ?? string.Empty;
            string nombreComercio = (string)raiz.Element("Nombre") ?? string.Empty;

            foreach (XElement elSol in raiz.Elements("Solicitud"))
            {
                string numeroGuia = (string)elSol.Element("NumeroGuia") ?? "SOLICITUD_SIN_GUIA";
                try
                {
                    PedidoDto dto = LeerPedidoDto(elSol);
                    despachosValidos.Add(dto.MapToDomain(codigoComercio, nombreComercio));
                }
                catch (Exception ex)
                {
                    inconsistencias.Add(new RegistroInconsistencia(nombreArchivo, numeroGuia, RegistroInconsistencia.TipoDatoInvalido, ex.Message, DateTime.Now));
                }
            }
        }
        catch (Exception ex)
        {
            inconsistencias.Add(new RegistroInconsistencia(nombreArchivo, "GENERAL", RegistroInconsistencia.TipoDatoInvalido, ex.Message, DateTime.Now));
        }

        return despachosValidos;
    }

    private PedidoDto LeerPedidoDto(XElement elSol)
    {
        XElement elProg = elSol.Element("Programacion");
        ProgramacionDto progDto = null;
        if (elProg != null)
        {
            progDto = new ProgramacionDto
            {
                Inicio = DateTime.Parse((string)elProg.Element("Inicio") ?? string.Empty, CultureInfo.InvariantCulture),
                Fin = DateTime.Parse((string)elProg.Element("Fin") ?? string.Empty, CultureInfo.InvariantCulture)
            };
        }

        PedidoDto dto = new PedidoDto
        {
            NumeroGuia = (string)elSol.Element("NumeroGuia") ?? string.Empty,
            Modalidad = (string)elSol.Element("Tipo") ?? string.Empty,
            Transportadora = (string)elSol.Element("Operador") ?? string.Empty,
            Destinatario = (string)elSol.Element("Cliente") ?? string.Empty,
            CiudadDestino = (string)elSol.Element("Destino") ?? string.Empty,
            DistanciaKm = double.Parse((string)elSol.Element("Distancia") ?? "0", CultureInfo.InvariantCulture),
            Programacion = progDto
        };

        XElement elBultos = elSol.Element("Bultos");
        if (elBultos != null)
        {
            foreach (XElement elBulto in elBultos.Elements("Bulto"))
            {
                dto.Paquetes.Add(new PaqueteDto
                {
                    IdPaquete = (string)elBulto.Element("Referencia") ?? string.Empty,
                    Descripcion = (string)elBulto.Element("Descripcion") ?? string.Empty,
                    Peso = double.Parse((string)elBulto.Element("Peso") ?? "0", CultureInfo.InvariantCulture),
                    ValorDeclarado = decimal.Parse((string)elBulto.Element("Valor") ?? "0", CultureInfo.InvariantCulture)
                });
            }
        }

        return dto;
    }
}
