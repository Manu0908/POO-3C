namespace prueba
{
    public class TrabajoConvencional : Trabajo
    {
        private const decimal _precioPorPagina = 100;

        public TrabajoConvencional(string codigo, string documento, int paginas, int copias)
            : base(codigo, documento, paginas, copias) { }

        public override string Modalidad
        {
            get { return "Convencional"; }
        }

        public override decimal CalcularCosto()
        {
            return Paginas * Copias * _precioPorPagina;
        }
    }
}
