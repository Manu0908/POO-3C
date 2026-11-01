using System;

namespace prueba
{
    public abstract class Atencion
    {
        private string _codigo;
        private string _descripcion;

        protected Atencion(string codigo, string descripcion)
        {
            Codigo = codigo;
            Descripcion = descripcion;
        }

        public string Codigo
        {
            get { return _codigo; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new VeterinariaException("El código de la atención es obligatorio.");
                }
                _codigo = value.Trim();
            }
        }

        public string Descripcion
        {
            get { return _descripcion; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new VeterinariaException("La descripción de la atención es obligatoria.");
                }
                _descripcion = value.Trim();
            }
        }

        public abstract string Tipo { get; }

        public abstract double CalcularCosto();

        public void MostrarInfo()
        {
            Console.WriteLine($"  [{Codigo}] {Tipo} - {Descripcion}: {CalcularCosto():C0}");
        }
    }
}
