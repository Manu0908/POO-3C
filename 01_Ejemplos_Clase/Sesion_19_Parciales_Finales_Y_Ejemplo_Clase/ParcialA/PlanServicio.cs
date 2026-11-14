namespace ParcialA
{
    public abstract class PlanServicio
    {
        protected PlanServicio()
        {
        }

        public abstract string Nombre { get; }

        public abstract double CostoMensual { get; }

        public abstract string DescripcionBeneficios { get; }

        public void MostrarInformacion()
        {
            Console.WriteLine($"Plan: {Nombre}");
            Console.WriteLine($"Beneficios: {DescripcionBeneficios}");
            Console.WriteLine($"Costo plan: ${CostoMensual:N0}");
        }
    }
}
