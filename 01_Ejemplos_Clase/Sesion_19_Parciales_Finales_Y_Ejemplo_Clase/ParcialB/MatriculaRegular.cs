namespace ParcialB
{
    public class MatriculaRegular : IModalidadMatricula
    {
        public MatriculaRegular()
        {
        }

        public string Nombre
        {
            get { return "Matrícula regular"; }
        }

        public double CalcularValorFinal(double costoPrograma)
        {
            return costoPrograma;
        }
    }
}
