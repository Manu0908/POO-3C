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
            string codigoComercio = (string)raiz.Attribute("codigoComercio") ?? string.Empty;

            foreach (XElement elSol in raiz.Elements("solicitud"))
            {
                string numeroGuia = (string)elSol.Attribute("guia") ?? "SOLICITUD_SIN_GUIA";
                try
                {
                    PedidoDto dto = LeerPedidoDto(elSol);
                    despachosValidos.Add(dto.MapToDomain(codigoComercio));
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
        XElement elProg = elSol.Element("programacion");
        PedidoDto dto = new PedidoDto
        {
            NumeroGuia = (string)elSol.Attribute("guia") ?? string.Empty,
            Modalidad = (string)elSol.Attribute("modalidad") ?? string.Empty,
            Destinatario = (string)elSol.Element("destinatario") ?? string.Empty,
            CiudadDestino = (string)elSol.Element("ciudadDestino") ?? string.Empty,
            Transportadora = (string)elSol.Element("transportadora") ?? string.Empty,
            DistanciaKm = double.Parse((string)elSol.Element("distanciaKm") ?? "0", CultureInfo.InvariantCulture),
            FechaProgramada = elProg != null ? (string)elProg.Attribute("fecha") ?? string.Empty : string.Empty,
            HoraInicio = elProg != null ? (string)elProg.Attribute("horaInicio") ?? string.Empty : string.Empty,
            HoraFin = elProg != null ? (string)elProg.Attribute("horaFin") ?? string.Empty : string.Empty
        };

        XElement elPaquetes = elSol.Element("paquetes");
        if (elPaquetes != null)
        {
            foreach (XElement elPaq in elPaquetes.Elements("paquete"))
            {
                dto.Paquetes.Add(new PaqueteDto
                {
                    IdPaquete = (string)elPaq.Attribute("id") ?? string.Empty,
                    Descripcion = (string)elPaq.Element("descripcion") ?? string.Empty,
                    Peso = double.Parse((string)elPaq.Element("peso") ?? "0", CultureInfo.InvariantCulture),
                    ValorDeclarado = decimal.Parse((string)elPaq.Element("valorDeclarado") ?? "0", CultureInfo.InvariantCulture)
                });
            }
        }

        return dto;
    }
}
