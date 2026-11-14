namespace ParcialA
{
    public class PlanPlus : PlanServicio
    {
        private const double _costoBase = 90000;
        private const double _costoPorReserva = 25000;

        private int _reservas;

        private PlanPlus()
        {
        }

        public PlanPlus(int reservas)
        {
            Reservas = reservas;
        }

        public int Reservas
        {
            get { return _reservas; }
            private set
            {
                if (value < 0)
                {
                    throw new Exception($"El número de reservas del plan Plus no puede ser negativo (recibido: {value}).");
                }
                _reservas = value;
            }
        }

        public override string Nombre
        {
            get { return "Plus"; }
        }

        public override double CostoMensual
        {
            get { return _costoBase + Reservas * _costoPorReserva; }
        }

        public override string DescripcionBeneficios
        {
            get { return $"Zonas compartidas + {Reservas} reservas de sala"; }
        }
    }
}
