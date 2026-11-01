using System;

namespace prueba
{
    public class AireAcondicionado
    {
        private const int TemperaturaMinima = 16;
        private const int TemperaturaMaxima = 30;

        private string _numeroSerie;
        private int _temperatura;

        public AireAcondicionado(string numeroSerie, int temperatura)
        {
            NumeroSerie = numeroSerie;
            Temperatura = temperatura;
        }

        public string NumeroSerie
        {
            get { return _numeroSerie; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("El número de serie es obligatorio.");
                }

                _numeroSerie = value;
            }
        }

        public int Temperatura
        {
            get { return _temperatura; }
            private set
            {
                if (value < TemperaturaMinima || value > TemperaturaMaxima)
                {
                    throw new Exception(
                        $"La temperatura debe estar entre {TemperaturaMinima} °C y {TemperaturaMaxima} °C.");
                }

                _temperatura = value;
            }
        }

        public bool Encendido { get; private set; }

        public void Encender()
        {
            Encendido = true;
        }

        public void Apagar()
        {
            Encendido = false;
        }

        public void ModificarTemperatura(int temperatura)
        {
            if (!Encendido)
            {
                Console.WriteLine("No se puede cambiar la temperatura de un aire acondicionado apagado.");
                return;
            }

            Temperatura = temperatura;
        }
    }
}
