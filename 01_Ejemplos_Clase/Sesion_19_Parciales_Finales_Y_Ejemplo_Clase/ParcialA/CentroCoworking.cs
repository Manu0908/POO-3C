namespace ParcialA
{
    public class CentroCoworking
    {
        private string _nombre;
        private List<Usuario> _usuarios = [];

        private CentroCoworking()
        {
        }

        public CentroCoworking(string nombre)
        {
            _nombre = nombre;
        }

        public void RegistrarUsuario(Usuario usuario)
        {           

            if (ExisteIdentificacion(usuario.Identificacion))
            {
                throw new UsuarioDuplicadoException($"Ya existe un usuario registrado con la identificación {usuario.Identificacion}.");
            }

            _usuarios.Add(usuario);
        }

        private bool ExisteIdentificacion(string identificacion)
        {
            return _usuarios.Any(u => u.Identificacion == identificacion);
        }

        public double CalcularIngresoTotal()
        {
            return _usuarios.Sum(u => u.CalcularValorFinal());
        }

        public void GenerarReporte()
        {
            foreach (Usuario u in _usuarios)
            {
                u.ObtenerDetalle();
            }

            Console.WriteLine($"Ingreso total mensual: ${CalcularIngresoTotal():N0}");
        }
    }
}
