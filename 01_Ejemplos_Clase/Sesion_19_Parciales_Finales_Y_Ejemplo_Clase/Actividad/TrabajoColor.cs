namespace prueba
{
    public class TrabajoColor : Trabajo
    {
        private const decimal _precioPorPagina = 500;

        public TrabajoColor(string codigo, string documento, int paginas, int copias)
            : base(codigo, documento, paginas, copias) { }

        public override string Modalidad
        {
            get { return "Color"; }
        }

        public override decimal CalcularCosto()
        {
            return Paginas * Copias * _precioPorPagina;
        }
    }
}
