namespace prueba
{
    public class Hotel
    {
        private string _nombre;
        private readonly List<Habitacion> _habitaciones;

        public Hotel(string nombre)
        {
            Nombre = nombre;
            _habitaciones = new List<Habitacion>();
        }

        public string Nombre
        {
            get { return _nombre; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("El nombre del hotel es obligatorio.");
                }

                _nombre = value;
            }
        }

        public void RegistrarHabitacion(Habitacion habitacion)
        {
            if (habitacion is null)
            {
                Console.WriteLine("No se puede registrar una habitación nula.");
                return;
            }

            if (BuscarHabitacion(habitacion.Numero) is not null)
            {
                Console.WriteLine($"Ya existe una habitación con el número {habitacion.Numero}.");
                return;
            }

            _habitaciones.Add(habitacion);
        }

        private Habitacion? BuscarHabitacion(int numero)
        {
            return _habitaciones.FirstOrDefault(x => x.Numero == numero);
        }

        public int CantidadHabitacionesConAireEncendido()
        {
            int cantidad = 0;
            foreach (Habitacion habitacion in _habitaciones)
            {
                if (habitacion.AireEncendido())
                {
                    cantidad++;
                }
            }

            return cantidad;

            //Linq
            //return _habitaciones.Count(x => x.AireEncendido());
        }
    }
}
