namespace Actividad_sesion_4
{
    internal class Cine
    {
        static void Main(string[] args)
        {
            Pelicula pelicula1 = new Pelicula("83-HD","Pelicula X","Accion",168,54);
            Pelicula pelicula2 = new Pelicula("67-HBO","Pelicula Z","Comedia",67,120);

            Sala sala1 = new Sala("12",60,60,"Disponible");
            Sala sala2 = new Sala("10",80,45,"En Mantenimiento");

            pelicula1.MostrarReproducciones();

            pelicula1.RegistrarReproduccion();

            pelicula1.MostrarReproducciones();

            sala1.MostrarAsientosDisponibles();

            sala1.RegistrarIngreso();

            sala2.MostrarAsientosDisponibles();

            sala2.RegistrarIngreso();

            sala2.MostrarAsientosDisponibles();

            Console.WriteLine($"Sala: {sala2.NumeroID} ¿Esta llena?: {sala2.SalaLlena()}");
        }

        public class Pelicula
        {
            public string Codigo { get; private set; }
            public string Titulo { get; private set; }
            public string Genero { get; private set; }
            public int DuracionMinutos { get; private set; }
            public int Reproducciones { get; private set; }
            public bool EsLarga { get { return DuracionMinutos > 120; } }

            private Pelicula() { }
            public Pelicula(string codigo, string titulo, string genero, int duracionMinutos, int reproducciones)
            {
                Codigo = codigo;
                Titulo = titulo;
                Genero = genero;
                DuracionMinutos = duracionMinutos;
                Reproducciones = reproducciones;
            }

            public void RegistrarReproduccion()
            {
                Reproducciones++;
            }
            public void MostrarReproducciones()
            {
                Console.WriteLine($"Reproducciones de la pelicula: {Reproducciones}");
            }
        }

        public class Sala
        {
            public static double PorcentajeMaxOcupacion = 0.90;
            public string NumeroID { get; private set; }
            public int Capacidad { get; private set; }
            public int Aforo { get; private set; }
            public string Disponibilidad { get; private set; }

            private Sala() { }
            public Sala(string numeroID, int capacidad, int aforo, string disponibilidad)
            {
                NumeroID = numeroID;
                Capacidad = capacidad;
                Aforo = aforo;
                Disponibilidad = disponibilidad;
            }
            public void RegistrarIngreso()
            {
                if (!SalaLlena())
                {
                    Aforo++;
                }
                else
                {
                    Console.WriteLine("No se pudo registrar el ingreso. La sala esta llena.");
                }
            }
            public void RegistrarEgreso()
            {
                Aforo--;
            }
            public void MostrarAsientosDisponibles()
            {
                int disponibles = Capacidad - Aforo;
                Console.WriteLine($"Asientos Disponibles: {disponibles}");
            }
            public bool SalaLlena()
            {
                return Aforo >= (Capacidad * PorcentajeMaxOcupacion);
            }
        }
    }
}