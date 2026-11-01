using System;

namespace prueba
{
    public class Inscripcion
    {
        private const string EstadoPendiente = "Pendiente";
        private const string EstadoCompletado = "Completado";
        private const int NumeroCaracteresCodigo = 5;

        private int _numero;
        private string _codigoCurso;
        private string _documentoEstudiante;
        private DateTime _fechaInscripcion;
        private double _valorTotal;
        private double _valorPagado;
        private bool? _resultado;
        private DateTime? _fechaCertificacion;

        private Inscripcion()
        {
        }

        public Inscripcion(int numero, string codigoCurso, string documentoEstudiante,
            DateTime fechaInscripcion, double valorTotal, double cuotaInicial)
        {
            Numero = numero;
            CodigoCurso = codigoCurso;
            DocumentoEstudiante = documentoEstudiante;
            FechaInscripcion = fechaInscripcion;
            ValorTotal = valorTotal;
            ValorPagado = cuotaInicial;
        }

        public int Numero
        {
            get
            {
                return _numero;
            }
            private set
            {
                if (value <= 0)
                {
                    throw new Exception("El número de inscripción debe ser positivo.");
                }
                _numero = value;
            }
        }

        public string CodigoCurso
        {
            get
            {
                return _codigoCurso;
            }
            private set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Trim().Length != NumeroCaracteresCodigo)
                {
                    throw new Exception($"El código del curso debe tener exactamente {NumeroCaracteresCodigo} caracteres.");
                }
                _codigoCurso = value.Trim();
            }
        }

        public string DocumentoEstudiante
        {
            get
            {
                return _documentoEstudiante;
            }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("El documento del estudiante es obligatorio.");
                }
                _documentoEstudiante = value;
            }
        }

        public DateTime FechaInscripcion
        {
            get
            {
                return _fechaInscripcion;
            }
            private set
            {
                _fechaInscripcion = value;
            }
        }

        public double ValorTotal
        {
            get
            {
                return _valorTotal;
            }
            private set
            {
                if (value <= 0)
                {
                    throw new Exception("El valor total debe ser mayor que cero.");
                }
                _valorTotal = value;
            }
        }

        public double ValorPagado
        {
            get
            {
                return _valorPagado;
            }
            private set
            {
                if (value <= 0 || value > _valorTotal)
                {
                    throw new Exception("El valor pagado debe ser mayor que cero y no superar el valor total.");
                }
                _valorPagado = value;
            }
        }

        public bool? Resultado
        {
            get
            {
                return _resultado;
            }
            private set
            {
                _resultado = value;
            }
        }

        public DateTime? FechaCertificacion
        {
            get
            {
                return _fechaCertificacion;
            }
            private set
            {
                _fechaCertificacion = value;
            }
        }

        public double Saldo
        {
            get
            {
                return _valorTotal - _valorPagado;
            }
        }

        public string EstadoPago
        {
            get
            {
                if (_valorPagado >= _valorTotal)
                {
                    return EstadoCompletado;
                }
                return EstadoPendiente;
            }
        }

        public bool PendienteDeResultado
        {
            get
            {
                return !_resultado.HasValue;
            }
        }

        public bool SePuedeCertificar
        {
            get
            {
                return _resultado.HasValue && _resultado.Value && EstadoPago == EstadoCompletado;
            }
        }

        public void Abonar(double monto)
        {
            if (monto <= 0)
            {
                Console.WriteLine("El abono debe ser mayor que cero.");
                return;
            }
            if (_valorPagado + monto > _valorTotal)
            {
                Console.WriteLine("El abono no puede superar el saldo pendiente.");
                return;
            }
            ValorPagado = _valorPagado + monto;
        }

        public void RegistrarResultado(bool aprobo)
        {
            if (_resultado.HasValue)
            {
                Console.WriteLine("El resultado ya fue registrado y no se puede cambiar.");
                return;
            }

            Resultado = aprobo;
        }

        public void RegistrarCertificacion(DateTime fecha)
        {
            if (_fechaCertificacion.HasValue)
            {
                Console.WriteLine("La certificación ya fue registrada.");
                return;
            }
            if (!SePuedeCertificar)
            {
                Console.WriteLine("No se puede certificar: debe haber aprobado y tener el pago completado.");
                return;
            }
            if (fecha < _fechaInscripcion)
            {
                Console.WriteLine("La fecha de certificación no puede ser anterior a la de inscripción.");
                return;
            }
            FechaCertificacion = fecha;
        }
    }
}
