namespace ParcialB
{
    public interface IModalidadMatricula
    {
        string Nombre { get; }

        double CalcularValorFinal(double costoPrograma);
    }
}
