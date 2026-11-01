namespace prueba
{
    public class Consulta : Atencion
    {
        private const double ValorBase = 55000;

        public Consulta(string codigo, string descripcion) : base(codigo, descripcion)
        {
        }

        public override string Tipo
        {
            get { return "Consulta"; }
        }

        public override double CalcularCosto()
        {
            return ValorBase;
        }
    }
}
