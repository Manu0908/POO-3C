using System.Text.Json;

namespace prueba
{
    public class ExportadorJson : IExportador
    {
        public void Exportar(
            List<SolicitudReporteDto> dtos,
            string ruta)
        {
            var json = JsonSerializer.Serialize(dtos);
            File.WriteAllText(ruta, json);
        }
    }
}
