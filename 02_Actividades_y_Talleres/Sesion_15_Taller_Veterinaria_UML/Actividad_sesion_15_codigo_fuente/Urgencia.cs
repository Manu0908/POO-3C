namespace prueba
{
    public class Urgencia : Atencion
    {
        private const double ValorConsulta = 55000;
        private const double RecargoUrgencias = 20000;

        public Urgencia(string codigo, string descripcion) : base(codigo, descripcion)
        {
        }

        public override string Tipo
        {
            get { return "Urgencia"; }
        }

        public override double CalcularCosto()
        {
            return ValorConsulta + RecargoUrgencias;
        }
    }
}
