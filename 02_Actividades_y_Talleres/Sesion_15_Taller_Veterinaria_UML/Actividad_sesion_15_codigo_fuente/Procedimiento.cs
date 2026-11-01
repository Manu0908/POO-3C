namespace prueba
{
    public class Procedimiento : Atencion
    {
        private const double ValorBase = 90000;
        private const double ValorPorInsumo = 15000;

        private int _cantidadInsumos;

        public Procedimiento(string codigo, string descripcion, int cantidadInsumos) : base(codigo, descripcion)
        {
            CantidadInsumos = cantidadInsumos;
        }

        public int CantidadInsumos
        {
            get { return _cantidadInsumos; }
            private set
            {
                if (value < 0)
                {
                    throw new InsumosInvalidosException(value);
                }
                _cantidadInsumos = value;
            }
        }

        public override string Tipo
        {
            get { return "Procedimiento"; }
        }

        public override double CalcularCosto()
        {
            return ValorBase + ValorPorInsumo * CantidadInsumos;
        }
    }
}
