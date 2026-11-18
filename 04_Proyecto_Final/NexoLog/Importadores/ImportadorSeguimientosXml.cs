using System.Globalization;
using System.Text;
using System.Xml.Linq;
using NexoLog.DTOs;

namespace NexoLog.Importadores;

public class ImportadorSeguimientosXml : IImportadorArchivo<Acontecimiento>
{
    public List<Acontecimiento> Importar(string rutaArchivo, List<RegistroInconsistencia> inconsistencias)
    {
        List<Acontecimiento> acontecimientosValidos = new List<Acontecimiento>();
        string nombreArchivo = Path.GetFileName(rutaArchivo);

        try
        {
            string contenidoXml = File.ReadAllText(rutaArchivo, Encoding.UTF8);
            XDocument documento = XDocument.Parse(contenidoXml);

            foreach (XElement elEvt in documento.Root.Elements("Novedad"))
            {
                string idEvt = (string)elEvt.Element("Id") ?? "EVENTO_XML_INCOMPLETO";
                try
                {
                    AcontecimientoDto dto = new AcontecimientoDto
                    {
                        IdAcontecimiento = (string)elEvt.Element("Id") ?? string.Empty,
                        NumeroGuia = (string)elEvt.Element("Guia") ?? string.Empty,
                        Tipo = (string)elEvt.Element("Tipo") ?? string.Empty,
                        FechaHora = DateTime.Parse((string)elEvt.Element("Fecha") ?? string.Empty, CultureInfo.InvariantCulture)
                    };
                    acontecimientosValidos.Add(dto.MapToDomain());
                }
                catch (Exception ex)
                {
                    inconsistencias.Add(new RegistroInconsistencia(nombreArchivo, idEvt, RegistroInconsistencia.TipoDatoInvalido, ex.Message, DateTime.Now));
                }
            }
        }
        catch (Exception ex)
        {
            inconsistencias.Add(new RegistroInconsistencia(nombreArchivo, "GENERAL", RegistroInconsistencia.TipoDatoInvalido, ex.Message, DateTime.Now));
        }

        return acontecimientosValidos;
    }
}
