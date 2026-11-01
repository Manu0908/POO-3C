using System;

namespace prueba
{
    public class Habitacion
    {
        private int _capacidadMaxima;
        private AireAcondicionado _aireAcondicionado;

        public Habitacion(int numero, int capacidadMaxima, AireAcondicionado aireAcondicionado)
        {
            Numero = numero;
            CapacidadMaxima = capacidadMaxima;
            AireAcondicionado = aireAcondicionado;
        }

        public int Numero { get; private set; }

        public int CapacidadMaxima
        {
            get { return _capacidadMaxima; }
            private set
            {
                if (value <= 0)
                {
                    throw new Exception("La capacidad máxima debe ser mayor que cero.");
                }

                _capacidadMaxima = value;
            }
        }

        private AireAcondicionado AireAcondicionado
        {
            get { return _aireAcondicionado; }
            set
            {
                if (value is null)
                {
                    throw new Exception("Una habitación no puede existir sin un aire acondicionado asignado.");
                }

                _aireAcondicionado = value;
            }
        }

        public void EncenderAire()
        {
            _aireAcondicionado.Encender();
        }

        public void ApagarAire()
        {
            _aireAcondicionado.Apagar();
        }

        public void AjustarTemperatura(int temperatura)
        {
            _aireAcondicionado.ModificarTemperatura(temperatura);
        }

        public bool AireEncendido()
        {
            return _aireAcondicionado.Encendido;
        }

        public int TemperaturaAire()
        {
            return _aireAcondicionado.Temperatura;
        }
    }
}
