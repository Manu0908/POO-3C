using System;

namespace prueba
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Vehiculo");
            Vehiculo vehiculo = new Vehiculo("ABC123", "Toyota", "Corolla", 2022, 120000, 145000);
            double cotizacion = vehiculo.Cotizar(3);
            Console.WriteLine($"Cotización de 3 días: {cotizacion}");
            vehiculo.RegistrarAlquiler();
            vehiculo.RegistrarDevolucion(2000);
            double cotizacion7 = vehiculo.Cotizar(7);
            double cotizacion8 = vehiculo.Cotizar(8);
            Console.WriteLine($"Cotización de 7 días: {cotizacion7}");
            Console.WriteLine($"Cotización de 8 días: {cotizacion8}");
            vehiculo.RegistrarAlquiler();
            vehiculo.RegistrarDevolucion(4000);
            vehiculo.RegistrarAlquiler();
            vehiculo.VerInformacion();
            Console.WriteLine();
        }
    }
}
