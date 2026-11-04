using System;
using System.Collections.Generic;
using System.Text;
using TallerVehiculos.Dominio;

namespace Taller
{
	public class Empresa
	{
		private List<Vehiculo> _vehiculos = new List<Vehiculo>();

		public Empresa()
		{
			_vehiculos = new List<Vehiculo>();
		}

		public void IncorporarVehiculo(Vehiculo vehiculo)
		{
			if (vehiculo == null)
			{
				Console.WriteLine("No se puede incorporar un vehiculo nulo.");
				return;
			}

			foreach (var v in _vehiculos)
			{
				if (v.Codigo == vehiculo.Codigo)
				{
					Console.WriteLine($"Ya existe un vehiculo con el codigo {vehiculo.Codigo}.");
					return;
				}
			}

			_vehiculos.Add(vehiculo);
		}

		public void RetirarVehiculo(string codigo)
		{
			Vehiculo encontrado = null;
			foreach (var v in _vehiculos)
			{
				if (v.Codigo == codigo)
				{
					encontrado = v;
					break;
				}
			}

			if (encontrado == null)
			{
				Console.WriteLine($"No existe un vehiculo con el codigo {codigo}.");
				return;
			}
		}

		public void MostrarFlota()
		{
			if (_vehiculos.Count == 0)
			{
				Console.WriteLine("La flota esta vacia.");
				return;
			}

			foreach (var vehiculo in _vehiculos)
			{
				vehiculo.MostrarInfo();
			}
		}

		public double CalcularCostoTotal()
		{
			double total = 0;
			foreach (var vehiculo in _vehiculos)
			{
				total += vehiculo.CalcularValorAlquiler();
			}
			return total;
		}
	}
} 
