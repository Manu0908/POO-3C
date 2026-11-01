namespace prueba
{
    public interface ICobertura
    {
        string Nombre { get; }

        double CalcularBeneficio(double valorAcumulado);
    }
}
