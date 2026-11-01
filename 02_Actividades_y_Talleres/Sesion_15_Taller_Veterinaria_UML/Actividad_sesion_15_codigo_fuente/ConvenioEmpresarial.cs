using System;

namespace prueba
{
    public class ConvenioEmpresarial : ICobertura
    {
        private const double PorcentajeDescuento = 0.20;
        private const double DescuentoMaximo = 60000;

        public string Nombre
        {
            get { return "Convenio empresarial"; }
        }

        public double CalcularBeneficio(double valorAcumulado)
        {
            return Math.Min(valorAcumulado * PorcentajeDescuento, DescuentoMaximo);
        }
    }
}
