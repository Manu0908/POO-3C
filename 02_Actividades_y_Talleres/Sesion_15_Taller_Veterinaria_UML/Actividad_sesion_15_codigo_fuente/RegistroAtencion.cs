using System;

namespace prueba
{
    public class RegistroAtencion
    {
        private string _idPaciente;
        private string _nombrePaciente;
        private string _especie;
        private ICobertura _cobertura;
        private List<Atencion> _atenciones = new List<Atencion>();

        public RegistroAtencion(string idPaciente, string nombrePaciente, string especie, ICobertura cobertura)
        {
            IdPaciente = idPaciente;
            NombrePaciente = nombrePaciente;
            Especie = especie;
            Cobertura = cobertura;
            Cerrado = false;
        }

        public string IdPaciente
        {
            get { return _idPaciente; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new VeterinariaException("La identificación del paciente es obligatoria.");
                }
                _idPaciente = value.Trim();
            }
        }

        public string NombrePaciente
        {
            get { return _nombrePaciente; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new VeterinariaException("El nombre del paciente es obligatorio.");
                }
                _nombrePaciente = value.Trim();
            }
        }

        public string Especie
        {
            get { return _especie; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new VeterinariaException("La especie del paciente es obligatoria.");
                }
                _especie = value.Trim();
            }
        }

        private ICobertura Cobertura
        {
            get { return _cobertura; }
            set 
            {
                if (value == null)
                {
                    throw new VeterinariaException("El registro debe tener una modalidad de cobertura.");
                }
                _cobertura = value;
            }
        }

        public bool Cerrado { get; private set; }

        public double CostoAcumulado
        {
            get { return _atenciones.Sum(a => a.CalcularCosto()); }
        }

        public double Beneficio
        {
            get { return Cobertura.CalcularBeneficio(CostoAcumulado); }
        }

        public double TotalAPagar
        {
            get { return CostoAcumulado - Beneficio; }
        }

        public void AgregarAtencion(Atencion atencion)
        {
            if (Cerrado)
            {
                throw new RegistroCerradoException(IdPaciente);
            }
            if (atencion == null)
            {
                throw new VeterinariaException("La atención no puede ser nula.");
            }
            double valor = atencion.CalcularCosto();
            if (valor <= 0)
            {
                throw new ValorAtencionInvalidoException(atencion.Codigo, valor);
            }
            _atenciones.Add(atencion);
        }

        public void Cerrar()
        {
            if (Cerrado)
            {
                throw new RegistroCerradoException(IdPaciente);
            }
            if (_atenciones.Count == 0)
            {
                throw new RegistroSinAtencionesException(IdPaciente);
            }
            Cerrado = true;
        }

        public void MostrarResumen()
        {
            string estado = Cerrado ? "Cerrado" : "Abierto";
            Console.WriteLine($"Paciente: {NombrePaciente} ({Especie}) - Id: {IdPaciente} - {estado}");
            Console.WriteLine("Atenciones:");
            foreach (Atencion atencion in _atenciones)
            {
                atencion.MostrarInfo();
            }
            Console.WriteLine($"Costo acumulado: {CostoAcumulado:C0}");
            Console.WriteLine($"Cobertura: {Cobertura.Nombre}");
            Console.WriteLine($"Beneficio: {Beneficio:C0}");
            Console.WriteLine($"Total a pagar: {TotalAPagar:C0}");
        }
    }
}
