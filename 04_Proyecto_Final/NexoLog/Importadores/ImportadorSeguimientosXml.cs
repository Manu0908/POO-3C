using System.Globalization;
using System.Text;
using System.Xml.Linq;
using NexoLog.DTOs;

namespace NexoLog.Importadores;

public class ImportadorSeguimientosXml : IImportadorArchivo<Acontecimiento>
{
    public List<Acontecimiento> Importar(string rutaArchivo, List<RegistroInconsistencia> inconsistencias)
    {
        var acontecimientosValidos = new List<Acontecimiento>();

        try
        {
            string contenidoXml = File.ReadAllText(rutaArchivo, Encoding.UTF8);
            var documento = XDocument.Parse(contenidoXml);

            foreach (var elEvt in documento.Root.Elements("Novedad"))
            {
                string idEvt = ObtenerTexto(elEvt, "Id", "EVENTO_XML_INCOMPLETO");
                try
                {
                    var dto = new AcontecimientoDto
                    {
                        IdAcontecimiento = ObtenerTexto(elEvt, "Id", ""),
                        NumeroGuia = ObtenerTexto(elEvt, "Guia", ""),
                        Tipo = ObtenerTexto(elEvt, "Tipo", ""),
                        FechaHora = DateTime.Parse(ObtenerTexto(elEvt, "Fecha", ""), CultureInfo.InvariantCulture)
                    };
                    acontecimientosValidos.Add(Acontecimiento.MapToDomain(dto));
                }
                catch (Exception ex)
                {
                    inconsistencias.Add(new RegistroInconsistencia(rutaArchivo, idEvt, RegistroInconsistencia.TipoDatoInvalido, ex.Message, DateTime.Now));
                }
            }
        }
        catch (Exception ex)
        {
            inconsistencias.Add(new RegistroInconsistencia(rutaArchivo, "GENERAL", RegistroInconsistencia.TipoDatoInvalido, ex.Message, DateTime.Now));
        }

        return acontecimientosValidos;
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
