using TallerVehiculos.Dominio;
using TallerVehiculos.Servicios;

Bicicleta bicicleta = new Bicicleta("B-001");
PatinetaElectrica patineta = new PatinetaElectrica("P-001", 40);
AutomovilElectrico automovil = new AutomovilElectrico("A-001", 60);
EstacionCarga estacion = new EstacionCarga();
Empresa empresa = new Empresa();

empresa.IncorporarVehiculo(bicicleta);
empresa.IncorporarVehiculo(patineta);
empresa.IncorporarVehiculo(automovil);

Console.WriteLine("=== FLOTA REGISTRADA ===");
empresa.MostrarFlota();

Console.WriteLine("\n=== COSTOS DE ALQUILER POR 3 HORAS ===");
foreach (Vehiculo vehiculo in empresa.Vehiculos)
{
    Console.WriteLine($"{vehiculo.Codigo}: ${vehiculo.CalcularValorAlquiler(3):N0}");
}
Console.WriteLine($"Automovil por 5 horas: ${automovil.CalcularValorAlquiler(5):N0}");
Console.WriteLine($"Total de la flota por 3 horas: ${empresa.CalcularCostoTotal(3):N0}");

Console.WriteLine("\n=== RECARGAS VALIDAS ===");
estacion.Recargar(patineta, 30);
Console.WriteLine($"Patineta de 40% con 30 puntos: {patineta.NivelBateria}%");
estacion.Recargar(automovil, 25);
Console.WriteLine($"Automovil de 60% con 25 puntos: {automovil.NivelBateria}%");

Console.WriteLine("\n=== OPERACIONES INVALIDAS ===");
PatinetaElectrica patinetaPrueba = new PatinetaElectrica("P-002", 40);
try
{
    estacion.Recargar(patinetaPrueba, 70);
}
catch (Exception e)
{
    Console.WriteLine($"Operacion rechazada: {e.Message}");
}
Console.WriteLine($"Nivel conservado de la patineta P-002: {patinetaPrueba.NivelBateria}%");

try
{
    bicicleta.CalcularValorAlquiler(0);
}
catch (Exception e)
{
    Console.WriteLine($"Operacion rechazada: {e.Message}");
}

try
{
    empresa.IncorporarVehiculo(new Bicicleta("B-001"));
}
catch (Exception e)
{
    Console.WriteLine($"Operacion rechazada: {e.Message}");
}

try
{
    empresa.RetirarVehiculo("Z-999");
}
catch (Exception e)
{
    Console.WriteLine($"Operacion rechazada: {e.Message}");
}
Console.WriteLine($"Vehiculos en la flota tras los intentos invalidos: {empresa.Vehiculos.Count}");

Console.WriteLine("\n=== RETIRO DE UN VEHICULO ===");
empresa.RetirarVehiculo("B-001");
Console.WriteLine("Flota tras retirar B-001:");
empresa.MostrarFlota();
