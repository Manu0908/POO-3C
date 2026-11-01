using System;

namespace prueba
{
    public class PersonalLimpieza : Empleado
    {
        private string _zonaAsignada;
        private int _habitacionesLimpiadas;

        public string ZonaAsignada
        {
            get { return _zonaAsignada; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("La zona asignada no puede estar vacía.");
                }
                _zonaAsignada = value;
            }
        }

        public int HabitacionesLimpiadas
        {
            get { return _habitacionesLimpiadas; }
            private set
            {
                if (value < 0)
                {
                    throw new Exception("La cantidad de habitaciones limpiadas no puede ser negativa.");
                }
                _habitacionesLimpiadas = value;
            }
        }

        public PersonalLimpieza(int numeroIdentificacion, string nombreCompleto, double salarioBasico,
            DateTime fechaContratacion, string zonaAsignada)
            : base(numeroIdentificacion, nombreCompleto, salarioBasico, fechaContratacion)
        {
            ZonaAsignada = zonaAsignada;
            HabitacionesLimpiadas = 0;
        }

        public void LimpiarHabitacion()
        {
            HabitacionesLimpiadas++;
        }
    }
}
