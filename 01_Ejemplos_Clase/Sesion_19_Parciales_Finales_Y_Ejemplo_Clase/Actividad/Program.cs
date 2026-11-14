using prueba;

var laura = new Cliente("1045789632", "Laura Castro");
var sol = new Solicitud(1001, laura);
sol.AgregarTrabajo(new TrabajoConvencional("T1", "Tesis.pdf", 120, 2));
sol.AgregarTrabajo(new TrabajoColor("T2", "Afiche.pdf", 10, 5));

List<SolicitudReporteDto> dtos =
    sol.MapToDto();

Console.Write("Formato (json / csv): ");
string formato = Console.ReadLine();

IExportador exportador =
    ExportadorFactory.Crear(formato);

exportador.Exportar(dtos,
    $"solicitud.{formato}");

Console.WriteLine($"Solicitud exportada en formato {formato}.");
