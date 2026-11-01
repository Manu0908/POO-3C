using System;

namespace prueba
{
    public class Recepcionista : Empleado
    {
        private string _idiomaPrincipal;
        private int _huespedesAtendidos;

        public string IdiomaPrincipal
        {
            get { return _idiomaPrincipal; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("El idioma principal no puede estar vacío.");
                }
                _idiomaPrincipal = value;
            }
        }

        public int HuespedesAtendidos
        {
            get { return _huespedesAtendidos; }
            private set
            {
                if (value < 0)
                {
                    throw new Exception("La cantidad de huéspedes atendidos no puede ser negativa.");
                }
                _huespedesAtendidos = value;
            }
        }

        public Recepcionista(int numeroIdentificacion, string nombreCompleto, double salarioBasico,
            DateTime fechaContratacion, string idiomaPrincipal)
            : base(numeroIdentificacion, nombreCompleto, salarioBasico, fechaContratacion)
        {
            IdiomaPrincipal = idiomaPrincipal;
            HuespedesAtendidos = 0;
        }

        public void AtenderHuesped()
        {
            HuespedesAtendidos++;
        }
    }
}
