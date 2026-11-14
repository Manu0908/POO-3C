namespace prueba
{
    public class TrabajoFotografico : Trabajo
    {
        private const decimal _precioPorPagina = 1200;
        private const decimal _cargoPreparacion = 2500;

        public TrabajoFotografico(string codigo, string documento, int paginas, int copias)
            : base(codigo, documento, paginas, copias) { }

        public override string Modalidad
        {
            get { return "Fotográfica"; }
        }

        public override decimal CalcularCosto()
        {
            return Paginas * Copias * _precioPorPagina + _cargoPreparacion;
        }

        public override void ObtenerDetalle()
        {
            base.ObtenerDetalle();
            Console.WriteLine($"   Incluye preparación: ${_cargoPreparacion:N0}");
        }
    }
}
