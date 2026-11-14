namespace ParcialA
{
    public class TarifaRegular : IPoliticaComercial
    {
        public TarifaRegular()
        {
        }

        public string Nombre
        {
            get { return "Tarifa regular"; }
        }

        public double CalcularValorFinal(double costoPlan)
        {
            return costoPlan;
        }
    }
}
