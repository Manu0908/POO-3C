namespace ParcialB
{
    public class ProgramaIntensivo : ProgramaFormacion
    {
        private const double _costoMensual = 280000;

        private string _instrumento;

        private ProgramaIntensivo()
        {
        }

        public ProgramaIntensivo(string instrumento)
        {
            _instrumento = instrumento;
        }

        public override string Nombre
        {
            get { return "Intensivo"; }
        }

        public override double CostoMensual
        {
            get { return _costoMensual; }
        }

        public override string Descripcion
        {
            get { return $"Clases ilimitadas, instrumento en préstamo: {_instrumento}"; }
        }
    }
}
