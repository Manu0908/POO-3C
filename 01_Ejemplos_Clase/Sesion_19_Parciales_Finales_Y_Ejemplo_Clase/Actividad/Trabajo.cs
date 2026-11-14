namespace prueba
{
    public abstract class Trabajo
    {
        private string _codigo;
        private string _documento;
        private int _paginas;
        private int _copias;

        protected Trabajo()
        {
        }

        protected Trabajo(string codigo, string documento, int paginas, int copias)
        {
            Codigo = codigo;
            Documento = documento;
            Paginas = paginas;
            Copias = copias;
        }

        public string Codigo
        {
            get { return _codigo; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("El código del trabajo no puede estar vacío.");
                }
                _codigo = value.Trim().ToUpper();
            }
        }

        public string Documento
        {
            get { return _documento; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("El nombre del documento no puede estar vacío.");
                }
                _documento = value.Trim();
            }
        }

        public int Paginas
        {
            get { return _paginas; }
            private set
            {
                if (value <= 0)
                {
                    throw new Exception($"El documento debe tener al menos una página (recibido: {value}).");
                }
                _paginas = value;
            }
        }

        public int Copias
        {
            get { return _copias; }
            private set
            {
                if (value <= 0)
                {
                    throw new Exception($"La cantidad de copias debe ser mayor que cero (recibido: {value}).");
                }
                _copias = value;
            }
        }

        public abstract string Modalidad { get; }

        public abstract decimal CalcularCosto();

        public virtual void ObtenerDetalle()
        {
            Console.WriteLine($"{Codigo} | {Documento} | {Modalidad} | Páginas: {Paginas} | Copias: {Copias} | Costo: ${CalcularCosto():N0}");
        }
    }
}
