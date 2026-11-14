namespace ParcialA
{
    public interface IPoliticaComercial
    {
        string Nombre { get; }

        double CalcularValorFinal(double costoPlan);
    }
}
