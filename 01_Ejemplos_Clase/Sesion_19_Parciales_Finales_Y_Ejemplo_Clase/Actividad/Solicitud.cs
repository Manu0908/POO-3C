using System.Collections.Generic;
using System.Linq;
using System;

namespace prueba
{
    public class Solicitud
    {
        private int _numero;
        private Cliente _cliente;
        private List<Trabajo> _trabajos = [];
        private bool _cerrada;

        private Solicitud()
        {
        }

        public Solicitud(int numero, Cliente cliente)
        {
            Numero = numero;
            Cliente = cliente;
            _cerrada = false;
        }

        public int Numero
        {
            get { return _numero; }
            private set
            {
                if (value <= 0)
                {
                    throw new Exception("El número de la solicitud debe ser mayor que cero.");
                }
                _numero = value;
            }
        }

        public Cliente Cliente
        {
            get { return _cliente; }
            private set
            {
                if (value == null)
                {
                    throw new Exception("La solicitud debe estar asociada a un cliente.");
                }
                _cliente = value;
            }
        }

        public bool Cerrada
        {
            get { return _cerrada; }
        }

        public string Estado
        {
            get { return _cerrada ? "Cerrada" : "Pendiente"; }
        }

        public List<SolicitudReporteDto> MapToDto()
        {
            return _trabajos
                .Select(t => new SolicitudReporteDto
                {
                    Numero = _numero,
                    Estado = Estado,
                    IdCliente = _cliente.Id,
                    Cliente = _cliente.Nombre,
                    Codigo = t.Codigo,
                    Documento = t.Documento,
                    Modalidad = t.Modalidad,
                    Paginas = t.Paginas,
                    Copias = t.Copias,
                    Costo = t.CalcularCosto(),
                    Total = ValorTotal
                }).ToList();
        }

        public void AgregarTrabajo(Trabajo trabajo)
        {
            if (_cerrada)
            {
                throw new SolicitudCerradaException($"La solicitud {Numero} está cerrada; no admite nuevos trabajos.");
            }
            if (trabajo == null)
            {
                throw new Exception("El trabajo no puede ser nulo.");
            }
            if (ExisteCodigo(trabajo.Codigo))
            {
                throw new CodigoDuplicadoException($"Ya existe un trabajo con el código {trabajo.Codigo} en la solicitud {Numero}.");
            }

            _trabajos.Add(trabajo);
        }

        public void Cerrar()
        {
            if (_cerrada)
            {
                throw new SolicitudCerradaException($"La solicitud {Numero} ya se encuentra cerrada.");
            }

            _cerrada = true;
        }

        private bool ExisteCodigo(string codigo)
        {
            return _trabajos.Any(t => t.Codigo == codigo);
        }

        public decimal ValorTotal
        {
            get { return _trabajos.Sum(t => t.CalcularCosto()); }
        }

        public void GenerarReporte()
        {
            Console.WriteLine("========================================");
            Console.WriteLine($"Solicitud N.º {Numero} - Estado: {Estado}");
            Console.WriteLine($"Cliente: {Cliente.Id} - {Cliente.Nombre}");
            Console.WriteLine("----------------------------------------");

            if (_trabajos.Count == 0)
            {
                Console.WriteLine("Sin trabajos registrados.");
            }
            
            foreach (Trabajo t in _trabajos)
            {
                t.ObtenerDetalle();
            }

            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"Valor total: ${ValorTotal:N0}");
            Console.WriteLine("========================================");
        }
    }
}
