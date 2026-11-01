namespace prueba
{
    public class Vehiculo
    {
        private const string EstadoDisponible = "Disponible";
        private const string EstadoAlquilado = "Alquilado";
        private const string EstadoEnMantenimiento = "En mantenimiento";

        private const int LimiteKilometraje = 150000;
        private const int DiasParaDescuento = 7;
        private const double PorcentajeDescuento = 0.10;

        private string _placa;
        private string _marca;
        private string _modelo;
        private int _year;
        private double _tarifaDiaria;
        private int _kilometraje;

        public string Placa
        {
            get { return _placa; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("La placa es obligatoria");
                }
                _placa = value.Trim();
            }
        }

        public string Marca
        {
            get { return _marca; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("La marca es obligatoria.");
                }
                _marca = value.Trim();
            }
        }

        public string Modelo
        {
            get { return _modelo; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("El modelo es obligatorio.");
                }
                _modelo = value.Trim();
            }
        }

        public int Year
        {
            get { return _year; }
            private set
            {
                if (value <= 0)
                {
                    throw new Exception("El año debe ser mayor que cero.");
                }
                if (value > DateTime.Now.Year)
                {
                    throw new Exception("El año no puede ser mayor al año actual.");
                }
                _year = value;
            }
        }

        public double TarifaDiaria
        {
            get { return _tarifaDiaria; }
            private set
            {
                if (value <= 0)
                {
                    throw new Exception("Debe tener una tarifa diaria mayor que cero.");
                }
                _tarifaDiaria = value;
            }
        }

        public int Kilometraje
        {
            get { return _kilometraje; }
            private set
            {
                if (value < 0)
                {
                    throw new Exception("El kilometraje no puede ser negativo.");
                }
                if (value < _kilometraje)
                {
                    throw new Exception("El kilometraje debe acumularse");
                }
                _kilometraje = value;
            }
        }

        public string Estado { get; private set; }

        public bool EsAlquilable => Estado == EstadoDisponible;

        public bool NecesitaMantenimiento => Estado == EstadoEnMantenimiento;

        public int VecesAlquilado { get; private set; }

        public Vehiculo(string placa, string marca, string modelo, int year, double tarifaDiaria, int kilometrajeIngreso)
        {
            Placa = placa;
            Marca = marca;
            Modelo = modelo;
            Year = year;
            TarifaDiaria = tarifaDiaria;
            Estado = EstadoDisponible;
            Kilometraje = kilometrajeIngreso;
            VecesAlquilado = 0;
            AjustarEstadoSegunKilometraje();
        }

        public double Cotizar(int dias)
        {
            if (dias <= 0)
            {
                Console.WriteLine("La cantidad de días debe ser mayor que cero.");
                return 0;
            }

            double total = _tarifaDiaria * dias;

            if (dias > DiasParaDescuento)
            {
                total = total - (total * PorcentajeDescuento);
            }

            return total;
        }

        public void RegistrarAlquiler()
        {
            if (Estado == EstadoEnMantenimiento)
            {
                Console.WriteLine("El vehículo esta en mantenimiento y no puede alquilarse.");
                return;
            }
            if (Estado == EstadoAlquilado)
            {
                Console.WriteLine("El vehículo ya esta alquilado");
                return;
            }

            Estado = EstadoAlquilado;
            VecesAlquilado = VecesAlquilado + 1;
        }

        public void RegistrarDevolucion(int kilometrosRecorridos)
        {
            if (Estado != EstadoAlquilado)
            {
                Console.WriteLine("No se puede regresar un vehiculo no alquilado");
                return;
            }
            if (kilometrosRecorridos < 0)
            {
                Console.WriteLine("Los kilómetros recorridos no pueden ser negativos");
                return;
            }

            Kilometraje = _kilometraje + kilometrosRecorridos;
            Estado = EstadoDisponible;
            AjustarEstadoSegunKilometraje();
        }

        public void CambiarTarifaDiaria(double nuevaTarifa)
        {
            TarifaDiaria = nuevaTarifa;
        }

        public void VerInformacion()
        {
            Console.WriteLine($"La placa es: {_placa}");
            Console.WriteLine($"Marca: {Marca}");
            Console.WriteLine($"Modelo: {Modelo}");
            Console.WriteLine($"Año: {Year}");
            Console.WriteLine($"Tarifa diaria: {_tarifaDiaria}"); 
            Console.WriteLine($"Kilometraje: {_kilometraje} km");
            Console.WriteLine($"Estado: {Estado}");
            Console.WriteLine($"Veces alquilado: {VecesAlquilado}");
        }

        private void AjustarEstadoSegunKilometraje()
        {
            if (_kilometraje > LimiteKilometraje)
            {
                Estado = EstadoEnMantenimiento;
            }
        }
    }
}
