namespace prueba
{
    public class Cliente
    {
        private string _identificacion;
        private string _nombre;

        private Cliente()
        {
        }

        public Cliente(string identificacion, string nombre)
        {
            Id = identificacion;
            Nombre = nombre;
        }

        public string Id
        {
            get { return _identificacion; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("La identificación del cliente no puede estar vacía.");
                }
                _identificacion = value.Trim();
            }
        }

        public string Nombre
        {
            get { return _nombre; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("El nombre del cliente no puede estar vacío.");
                }
                _nombre = value.Trim();
            }
        }
    }
}
