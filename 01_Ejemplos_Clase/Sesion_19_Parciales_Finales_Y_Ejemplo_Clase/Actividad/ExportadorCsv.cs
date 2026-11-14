using System.Globalization;
using CsvHelper;

namespace prueba
{
    public class ExportadorCsv : IExportador
    {
        public void Exportar(
            List<SolicitudReporteDto> dtos,
            string ruta)
        {
            using var writer = new StreamWriter(ruta);
            using var csv = new CsvWriter(writer,
                CultureInfo.InvariantCulture);
            csv.WriteRecords(dtos);
        }
    }
}
