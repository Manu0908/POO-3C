namespace ParcialB
{
    public class Academia
    {
        private string _nombre;
        private List<Estudiante> _estudiantes = [];

        private Academia()
        {
        }

        public Academia(string nombre)
        {
            _nombre = nombre;
        }

        public void RegistrarEstudiante(Estudiante estudiante)
        {
            if (estudiante == null)
            {
                throw new Exception("el estudiante no puede ser nulo");
            }

            if (ExisteCodigo(estudiante.Codigo))
            {
                throw new EstudianteDuplicadoException($"ya existe un estudiante con el código {estudiante.Codigo}");
            }

            _estudiantes.Add(estudiante);
        }

        private bool ExisteCodigo(string codigo)
        {
            return _estudiantes.Any(e => e.Codigo == codigo);
        }

        public double CalcularIngresoTotal()
        {
            return _estudiantes.Sum(e => e.CalcularValorFinal());
        }

        public void GenerarReporte()
        {
            foreach (Estudiante e in _estudiantes)
            {
                e.ObtenerDetalle();
            }

            Console.WriteLine($"Ingreso total mensual: ${CalcularIngresoTotal():N0}");
        }
    }
}
