namespace ParcialB
{
    public abstract class ProgramaFormacion
    {
        protected ProgramaFormacion()
        {
        }

        public abstract string Nombre { get; }

        public abstract double CostoMensual { get; }

        public abstract string Descripcion { get; }

        public void MostrarInformacion()
        {
            Console.WriteLine($"Programa: {Nombre}");
            Console.WriteLine($"Incluye: {Descripcion}");
            Console.WriteLine($"Costo del programa: ${CostoMensual:N0}");
        }
    }
}
