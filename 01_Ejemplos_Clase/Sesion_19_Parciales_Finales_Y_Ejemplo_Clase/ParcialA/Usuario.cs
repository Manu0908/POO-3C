namespace ParcialA
{
    public class Usuario
    {
        private string _nombre;
        private string _identificacion;
        private PlanServicio _plan;
        private IPoliticaComercial _politica;

        private Usuario()
        {
        }

        public Usuario(string nombre, string identificacion, PlanServicio plan, IPoliticaComercial politica)
        {
            Nombre = nombre;
            Identificacion = identificacion;
            _plan = plan;
            _politica = politica;
        }

        public string Nombre
        {
            get { return _nombre; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("El nombre del usuario no puede estar vacío.");
                }
                _nombre = value.Trim();
            }
        }

        public string Identificacion
        {
            get { return _identificacion; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("La identificación del usuario no puede estar vacía.");
                }
                _identificacion = value.Trim();
            }
        }

        public void CambiarPolitica(IPoliticaComercial nuevaPolitica)
        {
            _politica = nuevaPolitica;
        }

        public double CalcularValorFinal()
        {
            return _politica.CalcularValorFinal(_plan.CostoMensual);
        }

        public void ObtenerDetalle()
        {
            Console.WriteLine($"Usuario: {_nombre} ({_identificacion})");
            _plan.MostrarInformacion();
            Console.WriteLine($"Política: {_politica.Nombre}");
            Console.WriteLine($"Valor final: ${CalcularValorFinal():N0}");
            Console.WriteLine();
        }
    }
}
