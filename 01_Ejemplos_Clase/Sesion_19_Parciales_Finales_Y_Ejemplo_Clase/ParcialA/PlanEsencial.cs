namespace ParcialA
{
    public class PlanEsencial : PlanServicio
    {
        private const double _costoMensual = 90000;

        public PlanEsencial()
        {
        }

        public override string Nombre
        {
            get { return "Esencial"; }
        }

        public override double CostoMensual
        {
            get { return _costoMensual; }
        }

        public override string DescripcionBeneficios
        {
            get { return "Zonas de trabajo compartidas"; }
        }
    }
}
