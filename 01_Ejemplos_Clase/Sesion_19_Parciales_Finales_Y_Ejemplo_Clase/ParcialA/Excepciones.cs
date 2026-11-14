namespace ParcialA
{
    public class UsuarioDuplicadoException : Exception
    {
        public UsuarioDuplicadoException(string mensaje) : base(mensaje) { }
    }
}
