using Taller;
using TallerVehiculos.Contratos;
using TallerVehiculos.Dominio;
using TallerVehiculos.Servicios;

var estacion = new EstacionCarga();

Console.WriteLine("=== COSTOS DE ALQUILER ===");

var bicicleta3h = new Bicicleta("B-001", 3);
var patineta3h = new PatinetaElectrica("P-001", 3);
var automovil3h = new AutomovilElectrico("A-001", 3);
var automovil6h = new AutomovilElectrico("A-002", 6);

Console.WriteLine($"Bicicleta 3h: ${bicicleta3h.CalcularValorAlquiler():N0}");
Console.WriteLine($"Patineta 3h: ${patineta3h.CalcularValorAlquiler():N0}");
Console.WriteLine($"Automovil 3h: ${automovil3h.CalcularValorAlquiler():N0}");
Console.WriteLine($"Automovil 6h: ${automovil6h.CalcularValorAlquiler():N0}");

Console.WriteLine("\n=== RECARGAS VALIDAS ===");

estacion.Recargar(patineta3h, 70);
Console.WriteLine($"Patineta cargada a: {patineta3h.NivelBateria}%");

estacion.Recargar(patineta3h, 20);
Console.WriteLine($"Patineta tras +20: {patineta3h.NivelBateria}%");

estacion.Recargar(automovil3h, 60);
Console.WriteLine($"Automovil cargado a: {automovil3h.NivelBateria}%");

estacion.Recargar(automovil3h, 36);
Console.WriteLine($"Automovil tras +36: {automovil3h.NivelBateria}%");

Console.WriteLine("\n=== RECARGAS INVALIDAS ===");

var patineta70 = new PatinetaElectrica("P-002", 2);
estacion.Recargar(patineta70, 70);
Console.WriteLine($"Patineta en {patineta70.NivelBateria}% antes del intento invalido.");
estacion.Recargar(patineta70, 40);
Console.WriteLine($"Patineta tras intento invalido: {patineta70.NivelBateria}%");

Console.WriteLine("\n=== ALQUILER POR 0 HORAS ===");

try
{
	var invalido = new Bicicleta("X-001", 0);
}
catch (Exception e)
{
	Console.WriteLine($"Operacion rechazada: {e.Message}");
}

Console.WriteLine("\n=== FLOTA ===");

var flota = new Empresa();
flota.IncorporarVehiculo(bicicleta3h);
flota.IncorporarVehiculo(patineta3h);
flota.IncorporarVehiculo(automovil3h);
flota.IncorporarVehiculo(automovil6h);

Console.WriteLine("Flota completa:");
flota.MostrarFlota();

Console.WriteLine("\nIntento de incorporar codigo duplicado:");
flota.IncorporarVehiculo(new Bicicleta("B-001", 1));

Console.WriteLine("\nRetiro de un vehiculo:");
flota.RetirarVehiculo("A-002");
Console.WriteLine("Flota tras retiro:");
flota.MostrarFlota();

Console.WriteLine("\nIntento de retirar vehiculo inexistente:");
flota.RetirarVehiculo("Z-999");
