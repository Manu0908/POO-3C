namespace ParcialB
{
    public class ProgramaGrupal : ProgramaFormacion
    {
        private const double _costoMensual = 120000;

        public ProgramaGrupal()
        {
        }

        public override string Nombre
        {
            get { return "Grupal"; }
        }

        public override double CostoMensual
        {
            get { return _costoMensual; }
        }

        public override string Descripcion
        {
            get { return "Clases grupales semanales"; }
        }
    }
}
