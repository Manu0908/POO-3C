namespace ParcialA
{
    public class PlanEmpresarial : PlanServicio
    {
        private const double _costoMensual = 210000;

        private int _numeroEscritorio;

        private PlanEmpresarial()
        {
        }

        public PlanEmpresarial(int numeroEscritorio)
        {
            NumeroEscritorio = numeroEscritorio;
        }

        public int NumeroEscritorio
        {
            get { return _numeroEscritorio; }
            private set
            {                
                _numeroEscritorio = value;
            }
        }

        public override string Nombre
        {
            get { return "Empresarial"; }
        }

        public override double CostoMensual
        {
            get { return _costoMensual; }
        }

        public override string DescripcionBeneficios
        {
            get { return $"Acceso ilimitado, escritorio exclusivo #{NumeroEscritorio}"; }
        }
    }
}
