namespace prueba
{
    public class SolicitudReporteDto
    {
        public int Numero { get; set; }
        public string Estado { get; set; }
        public string IdCliente { get; set; }
        public string Cliente { get; set; }
        public string Codigo { get; set; }
        public string Documento { get; set; }
        public string Modalidad { get; set; }
        public int Paginas { get; set; }
        public int Copias { get; set; }
        public decimal Costo { get; set; }
        public decimal Total { get; set; }
    }
}
