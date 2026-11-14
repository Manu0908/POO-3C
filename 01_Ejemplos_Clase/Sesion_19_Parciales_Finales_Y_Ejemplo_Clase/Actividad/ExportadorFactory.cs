namespace prueba
{
    public static class ExportadorFactory
    {
        public static IExportador Crear(
            string formato)
        {
            return formato switch
            {
                "json" => new ExportadorJson(),
                "csv" => new ExportadorCsv(),
                _ => throw new Exception(
                        "Formato no soportado")
            };
        }
    }
}
