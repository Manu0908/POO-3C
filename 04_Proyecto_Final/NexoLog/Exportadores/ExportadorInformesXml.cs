using System.Text;
using System.Xml.Serialization;
using NexoLog.DTOs;

namespace NexoLog.Exportadores;

public class ExportadorInformesXml : IExportadorInforme
{
    public string TipoArchivo => "xml";

    public void Exportar(InformeConsolidadoDto dto)
    {
        var serializer = new XmlSerializer(typeof(InformeConsolidadoDto));
        using var escritor = new StreamWriter("informes_nexolog.xml", false, Encoding.UTF8);
        serializer.Serialize(escritor, dto);
    }
}
