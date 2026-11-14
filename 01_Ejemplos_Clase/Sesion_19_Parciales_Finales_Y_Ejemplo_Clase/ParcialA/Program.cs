using ParcialA;

CentroCoworking centro = new CentroCoworking("WorkSpace");
Usuario laura = new Usuario("Laura Martínez", "1001", new PlanEsencial(), new TarifaRegular());
Usuario carlos = new Usuario("Carlos Pérez", "1002", new PlanPlus(2), new TarifaPromocional());
Usuario ana = new Usuario("Ana Gómez", "1003", new PlanEmpresarial(12), new TarifaRegular());

centro.RegistrarUsuario(laura);
centro.RegistrarUsuario(carlos);
centro.RegistrarUsuario(ana);

Console.WriteLine("--- Reporte inicial ---");
centro.GenerarReporte();

// ------------------------------------------------------------------- //
Console.WriteLine();

laura.CambiarPolitica(new TarifaPromocional());

Console.WriteLine("--- Después de cambiar la política de Laura ---");

centro.GenerarReporte();

// ------------------------------------------------------------------- //
Console.WriteLine();

Console.WriteLine("--- Validaciones ---");

// caso 1 
try
{
    centro.RegistrarUsuario(new Usuario("", "1004", new PlanEsencial(), new TarifaRegular()));
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

// caso 2 
try
{
    PlanPlus planInvalido = new PlanPlus(-1);
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

// caso 3
try
{
    centro.RegistrarUsuario(new Usuario("Pedro Ruiz", "1001", new PlanEsencial(), new TarifaRegular()));
}
catch (UsuarioDuplicadoException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
