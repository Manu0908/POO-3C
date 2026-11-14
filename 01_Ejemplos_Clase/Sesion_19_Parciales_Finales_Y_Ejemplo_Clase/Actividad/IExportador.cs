namespace prueba
{
    public interface IExportador
    {
        void Exportar(
            List<SolicitudReporteDto> dtos,
            string ruta);
    }
}
