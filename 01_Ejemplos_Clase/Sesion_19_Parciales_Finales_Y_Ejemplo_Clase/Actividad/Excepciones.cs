namespace prueba
{
    public class CodigoDuplicadoException : Exception
    {
        public CodigoDuplicadoException(string mensaje) : base(mensaje) { }
    }
    public class SolicitudCerradaException : Exception
    {
        public SolicitudCerradaException(string mensaje) : base(mensaje) { }
    }
}
