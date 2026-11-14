namespace ParcialB
{
    public class Estudiante
    {
        private string _nombre;
        private string _codigo;
        private ProgramaFormacion _programa;
        private IModalidadMatricula _modalidad;

        private Estudiante()
        {
        }

        public Estudiante(string nombre, string codigo, ProgramaFormacion programa, IModalidadMatricula modalidad)
        {
            Nombre = nombre;
            Codigo = codigo;
            _programa = programa;
            _modalidad = modalidad;
        }

        public string Nombre
        {
            get { return _nombre; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("el nombre y el código son obligatorios");
                }
                _nombre = value.Trim();
            }
        }

        public string Codigo
        {
            get { return _codigo; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("el nombre y el código son obligatorios");
                }
                _codigo = value.Trim();
            }
        }

        public void CambiarModalidad(IModalidadMatricula nuevaModalidad)
        {
            _modalidad = nuevaModalidad;
        }

        public double CalcularValorFinal()
        {
            return _modalidad.CalcularValorFinal(_programa.CostoMensual);
        }

        public void ObtenerDetalle()
        {
            Console.WriteLine($"Estudiante: {_nombre} ({_codigo})");
            _programa.MostrarInformacion();
            Console.WriteLine($"Modalidad: {_modalidad.Nombre}");
            Console.WriteLine($"Valor final: ${CalcularValorFinal():N0}");
            Console.WriteLine();
        }
    }
}
