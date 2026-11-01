using System.Globalization;
using System.Text;
using System.Xml.Linq;
using NexoLog.Dominio;
using NexoLog.Dominio.Inconsistencias;

namespace NexoLog.Infraestructura.Lectores;

/// <summary>
/// Lector resiliente para acontecimientos y eventos de seguimiento en formato XML.
/// </summary>
public class LectorSeguimientosXml : ILectorArchivo<Acontecimiento>
{
    public List<Acontecimiento> Leer(string rutaArchivo, List<RegistroInconsistencia> inconsistencias)
    {
        List<Acontecimiento> acontecimientosValidos = new List<Acontecimiento>();
        string nombreArchivo = Path.GetFileName(rutaArchivo);

        if (!File.Exists(rutaArchivo))
        {
            inconsistencias.Add(new RegistroInconsistencia(
                nombreArchivo,
                "GENERAL",
                TipoInconsistencia.ReferenciaInexistente,
                $"El archivo XML de seguimiento '{rutaArchivo}' no existe.",
                DateTime.Now));
            return acontecimientosValidos;
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
                        "El archivo XML de seguimiento no contiene elemento raíz.",
                        DateTime.Now));
                    return acontecimientosValidos;
                }

                IEnumerable<XElement> elementosAcontecimiento = raiz.Elements("acontecimiento");
                foreach (XElement elEvt in elementosAcontecimiento)
                {
                    string idEvt = (string)elEvt.Attribute("id") ?? string.Empty;
                    string guia = (string)elEvt.Attribute("guia") ?? string.Empty;
                    string tipoTexto = (string)elEvt.Attribute("tipo") ?? string.Empty;
                    string fechaHoraTexto = (string)elEvt.Attribute("fechaHora") ?? string.Empty;

                    if (string.IsNullOrWhiteSpace(idEvt) || string.IsNullOrWhiteSpace(guia))
                    {
                        inconsistencias.Add(new RegistroInconsistencia(
                            nombreArchivo,
                            "EVENTO_XML_INCOMPLETO",
                            TipoInconsistencia.DatoInvalido,
                            "Se detectó un elemento <acontecimiento> sin atributo 'id' o 'guia'.",
                            DateTime.Now));
                        continue;
                    }

                    if (!Enum.TryParse(tipoTexto, true, out TipoAcontecimiento tipoAcontecimiento))
                    {
                        inconsistencias.Add(new RegistroInconsistencia(
                            nombreArchivo,
                            idEvt,
                            TipoInconsistencia.DatoInvalido,
                            $"Tipo de acontecimiento XML desconocido '{tipoTexto}' en el evento '{idEvt}'.",
                            DateTime.Now));
                        continue;
                    }

                    if (!DateTime.TryParse(fechaHoraTexto, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaHora))
                    {
                        inconsistencias.Add(new RegistroInconsistencia(
                            nombreArchivo,
                            idEvt,
                            TipoInconsistencia.DatoInvalido,
                            $"Fecha/Hora XML inválida '{fechaHoraTexto}' en el evento '{idEvt}'.",
                            DateTime.Now));
                        continue;
                    }

                    try
                    {
                        Acontecimiento nuevoAcontecimiento = new Acontecimiento(
                            idEvt,
                            guia,
                            tipoAcontecimiento,
                            fechaHora);

                        acontecimientosValidos.Add(nuevoAcontecimiento);
                    }
                    catch (NexoLogException ex)
                    {
                        inconsistencias.Add(new RegistroInconsistencia(
                            nombreArchivo,
                            idEvt,
                            TipoInconsistencia.DatoInvalido,
                            $"Error al crear acontecimiento XML '{idEvt}': {ex.Message}",
                            DateTime.Now));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            inconsistencias.Add(new RegistroInconsistencia(
                nombreArchivo,
                "GENERAL",
                TipoInconsistencia.DatoInvalido,
                $"Error crítico al procesar el archivo XML de seguimiento: {ex.Message}",
                DateTime.Now));
        }

        return acontecimientosValidos;
    }
}
