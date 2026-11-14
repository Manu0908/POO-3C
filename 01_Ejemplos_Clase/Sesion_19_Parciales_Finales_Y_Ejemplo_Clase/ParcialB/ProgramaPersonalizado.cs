namespace ParcialB
{
    public class ProgramaPersonalizado : ProgramaFormacion
    {
        private const double _costoBase = 120000;
        private const double _costoPorClase = 35000;

        private int _clasesIndividuales;

        private ProgramaPersonalizado()
        {
        }

        public ProgramaPersonalizado(int clasesIndividuales)
        {
            ClasesIndividuales = clasesIndividuales;
        }

        public int ClasesIndividuales
        {
            get { return _clasesIndividuales; }
            private set
            {
                if (value < 0)
                {
                    throw new Exception($"el número de clases individuales no puede ser negativo ({value})");
                }
                _clasesIndividuales = value;
            }
        }

        public override string Nombre
        {
            get { return "Personalizado"; }
        }

        public override double CostoMensual
        {
            get { return _costoBase + ClasesIndividuales * _costoPorClase; }
        }

        public override string Descripcion
        {
            get { return $"Clases grupales + {ClasesIndividuales} clases individuales"; }
        }
    }
}
