namespace ParcialA
{
    public class TarifaPromocional : IPoliticaComercial
    {
        private const double _porcentajeDescuento = 0.10;

        public TarifaPromocional()
        {
        }

        public string Nombre
        {
            get { return "Tarifa promocional"; }
        }

        public double CalcularValorFinal(double costoPlan)
        {
            return costoPlan - costoPlan * _porcentajeDescuento;
        }
    }
}
