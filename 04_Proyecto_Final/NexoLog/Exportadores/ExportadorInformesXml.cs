using System.Text;
using System.Xml.Serialization;
using NexoLog.DTOs;

namespace NexoLog.Exportadores;

public class ExportadorInformesXml : IExportadorInforme
{
    public string Extension { get { return "xml"; } }

    public void Exportar(InformeConsolidadoDto dto, string rutaDestino)
    {
        string directorio = Path.GetDirectoryName(rutaDestino);
        if (!string.IsNullOrWhiteSpace(directorio) && !Directory.Exists(directorio))
        {
            Directory.CreateDirectory(directorio);
        }

        XmlSerializer serializer = new XmlSerializer(typeof(InformeConsolidadoDto));
        using StreamWriter escritor = new StreamWriter(rutaDestino, false, Encoding.UTF8);
        serializer.Serialize(escritor, dto);
    }
}
