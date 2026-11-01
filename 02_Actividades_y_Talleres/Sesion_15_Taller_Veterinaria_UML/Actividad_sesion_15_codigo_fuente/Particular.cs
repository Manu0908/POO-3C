namespace prueba
{
    public class Particular : ICobertura
    {
        public string Nombre
        {
            get { return "Particular"; }
        }

        public double CalcularBeneficio(double valorAcumulado)
        {
            return 0;
        }
    }
}
