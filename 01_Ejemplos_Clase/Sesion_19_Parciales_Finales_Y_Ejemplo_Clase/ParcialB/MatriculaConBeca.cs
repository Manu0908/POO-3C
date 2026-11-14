namespace ParcialB
{
    public class MatriculaConBeca : IModalidadMatricula
    {
        private const double _porcentajeDescuento = 0.20;

        public MatriculaConBeca()
        {
        }

        public string Nombre
        {
            get { return "Matrícula con beca"; }
        }

        public double CalcularValorFinal(double costoPrograma)
        {
            return costoPrograma - costoPrograma * _porcentajeDescuento;
        }
    }
}
