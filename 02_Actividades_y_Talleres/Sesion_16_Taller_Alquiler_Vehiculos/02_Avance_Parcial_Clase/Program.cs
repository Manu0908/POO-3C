using TallerVehiculos.Dominio;
using TallerVehiculos.Servicios;

var bicicleta = new Bicicleta("B-001");
var patineta = new PatinetaElectrica("P-001");
var automovil = new AutomovilElectrico("A-001");

var flota = new List<Vehiculo>
{
    bicicleta,
    patineta,
    automovil,
    new Vehiculo("GEN-001", 10000m)
};

Console.WriteLine("=== ALQUILER POR 3 HORAS ===");
foreach (var vehiculo in flota)
{
    Console.WriteLine($"{vehiculo.Codigo}: ${vehiculo.CalcularCosto(3):N0}");
}

var estacion = new EstacionCarga();
Console.WriteLine("\n=== RECARGA ===");
Console.WriteLine($"Patineta antes: {patineta.NivelBateria}%");
estacion.Recargar(patineta, 70);
Console.WriteLine($"Patineta después: {patineta.NivelBateria}%");
