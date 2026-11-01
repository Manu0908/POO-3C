using System;

namespace prueba
{
    public class VeterinariaException : Exception
    {
        public VeterinariaException(string mensaje) : base(mensaje)
        {
        }
    }

    public class InsumosInvalidosException : VeterinariaException
    {
        public InsumosInvalidosException(int cantidad)
            : base($"La cantidad de insumos ({cantidad}) no es válida; debe ser mayor o igual a cero.")
        {
        }
    }

    public class ValorAtencionInvalidoException : VeterinariaException
    {
        public ValorAtencionInvalidoException(string codigo, double valor)
            : base($"La atención {codigo} tiene un valor inválido ({valor:C0}); debe ser mayor que cero.")
        {
        }
    }

    public class RegistroCerradoException : VeterinariaException
    {
        public RegistroCerradoException(string idPaciente)
            : base($"El registro del paciente {idPaciente} ya está cerrado; no se pueden agregar ni modificar atenciones.")
        {
        }
    }

    public class RegistroSinAtencionesException : VeterinariaException
    {
        public RegistroSinAtencionesException(string idPaciente)
            : base($"El registro del paciente {idPaciente} no tiene atenciones; no se puede cerrar.")
        {
        }
    }
}
