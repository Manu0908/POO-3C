using System;

namespace prueba
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Crear un curso: código de 5 caracteres, nombre, duración, matrícula y cupos.
            Curso curso = new Curso("PRG01", "Programacion", 40, 500000, 5);
            Console.WriteLine($"Curso {curso.Codigo} - {curso.Nombre}");
            Console.WriteLine($"Cupos disponibles: {curso.CuposDisponibles}, ¿Lleno?: {curso.EstaLleno}");

            // Calcular valor con y sin descuento por matrícula anticipada.
            double valorAnticipado = curso.CalcularValorInscripcion(true);
            double valorNormal = curso.CalcularValorInscripcion(false);
            Console.WriteLine($"Valor anticipado (10% dcto): {valorAnticipado}");
            Console.WriteLine($"Valor normal: {valorNormal}");

            // Ocupar un cupo e inscribir a un estudiante con una cuota inicial.
            curso.OcuparCupo();
            {
                Inscripcion inscripcion = new Inscripcion(
                    numero: 1,
                    codigoCurso: curso.Codigo,
                    documentoEstudiante: "1001",
                    fechaInscripcion: DateTime.Today,
                    valorTotal: valorAnticipado,
                    cuotaInicial: 200000);

                Console.WriteLine($"\nInscripción {inscripcion.Numero} creada.");
                Console.WriteLine($"Pagado: {inscripcion.ValorPagado}, Saldo: {inscripcion.Saldo}, Estado: {inscripcion.EstadoPago}");

                // Abonar al saldo.
                inscripcion.Abonar(250000);
                Console.WriteLine($"Tras abono -> Saldo: {inscripcion.Saldo}, Estado: {inscripcion.EstadoPago}");

                // Un abono que se pasa del total no se recibe.
                inscripcion.Abonar(1000000);

                // Completar el pago.
                inscripcion.Abonar(inscripcion.Saldo);
                Console.WriteLine($"Pago final -> Saldo: {inscripcion.Saldo}, Estado: {inscripcion.EstadoPago}");

                // Registrar resultado académico y certificar.
                Console.WriteLine($"¿Pendiente de resultado?: {inscripcion.PendienteDeResultado}");
                inscripcion.RegistrarResultado(true);
                Console.WriteLine($"Resultado: {inscripcion.Resultado}, ¿Se puede certificar?: {inscripcion.SePuedeCertificar}");

                inscripcion.RegistrarCertificacion(DateTime.Today);
                Console.WriteLine($"Fecha certificación: {inscripcion.FechaCertificacion}");
            }

            Console.WriteLine($"\nCupos disponibles ahora: {curso.CuposDisponibles}");
        }
    }
}
