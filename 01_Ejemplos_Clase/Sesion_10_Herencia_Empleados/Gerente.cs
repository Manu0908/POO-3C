using System;

namespace prueba
{
    public class Gerente : Empleado
    {
        private string _departamento;
        private double _presupuestoDisponible;

        public string Departamento
        {
            get { return _departamento; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("El departamento no puede estar vacío.");
                }
                _departamento = value;
            }
        }

        public double PresupuestoDisponible
        {
            get { return _presupuestoDisponible; }
            private set
            {
                if (value < 0)
                {
                    throw new Exception("El presupuesto disponible no puede ser negativo.");
                }
                _presupuestoDisponible = value;
            }
        }

        public Gerente(int numeroIdentificacion, string nombreCompleto, double salarioBasico,
            DateTime fechaContratacion, string departamento, double presupuestoDisponible)
            : base(numeroIdentificacion, nombreCompleto, salarioBasico, fechaContratacion)
        {
            Departamento = departamento;
            PresupuestoDisponible = presupuestoDisponible;
        }

        public bool AutorizarGasto(double monto)
        {
            if (monto <= 0)
            {
                throw new Exception("El monto del gasto debe ser mayor que cero.");
            }

            if (monto > PresupuestoDisponible)
            {
                return false;
            }

            PresupuestoDisponible -= monto;
            return true;
        }
    }
}
