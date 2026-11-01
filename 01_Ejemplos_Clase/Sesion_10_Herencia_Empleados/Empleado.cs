using System;

namespace prueba
{
    public class Empleado
    {
        private int _numeroIdentificacion;
        private string _nombreCompleto;
        private double _salarioBasico;
        private DateTime _fechaContratacion;

        public int NumeroIdentificacion
        {
            get { return _numeroIdentificacion; }
            private set
            {
                if (value <= 0)
                {
                    throw new Exception("El número de identificación debe ser mayor que cero.");
                }
                _numeroIdentificacion = value;
            }
        }

        public string NombreCompleto
        {
            get { return _nombreCompleto; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("El nombre completo no puede estar vacío.");
                }
                _nombreCompleto = value;
            }
        }

        public double SalarioBasico
        {
            get { return _salarioBasico; }
            private set
            {
                if (value < 0)
                {
                    throw new Exception("El salario no puede ser negativo.");
                }
                _salarioBasico = value;
            }
        }

        public DateTime FechaContratacion
        {
            get { return _fechaContratacion; }
            private set
            {
                if (value > DateTime.Today)
                {
                    throw new Exception("La fecha de contratación no puede ser posterior a la fecha actual.");
                }
                _fechaContratacion = value;
            }
        }

        public Empleado(int numeroIdentificacion, string nombreCompleto, double salarioBasico, DateTime fechaContratacion)
        {
            NumeroIdentificacion = numeroIdentificacion;
            NombreCompleto = nombreCompleto;
            SalarioBasico = salarioBasico;
            FechaContratacion = fechaContratacion;
        }

        public int CalcularAntiguedad()
        {
            int anios = DateTime.Today.Year - FechaContratacion.Year;
            return anios;
        }

        public void MostrarInformacion()
        {
            Console.WriteLine($"ID: {NumeroIdentificacion}");
            Console.WriteLine($"Nombre: {NombreCompleto}");
            Console.WriteLine($"Salario básico: {SalarioBasico:C}");
            Console.WriteLine($"Fecha de contratación: {FechaContratacion:d}");
            Console.WriteLine($"Antigüedad: {CalcularAntiguedad()} año(s)");
        }
    }
}
