using ParcialB;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Academia academia = new Academia("Compás");
Estudiante sofia = new Estudiante("Sofía Ramírez", "2001", new ProgramaGrupal(), new MatriculaRegular());
Estudiante andres = new Estudiante("Andrés Torres", "2002", new ProgramaPersonalizado(3), new MatriculaConBeca());
Estudiante valentina = new Estudiante("Valentina Ruiz", "2003", new ProgramaIntensivo("Violín"), new MatriculaRegular());

academia.RegistrarEstudiante(sofia);
academia.RegistrarEstudiante(andres);
academia.RegistrarEstudiante(valentina);

Console.WriteLine("--- Reporte inicial ---");
academia.GenerarReporte();

// ------------------------------------------------------------------- //
Console.WriteLine();

sofia.CambiarModalidad(new MatriculaConBeca());
Console.WriteLine("--- Después de cambiar la modalidad de Sofía ---");
academia.GenerarReporte();

// ------------------------------------------------------------------- //
Console.WriteLine();
Console.WriteLine("--- Validaciones ---");

// caso 1
try
{
    academia.RegistrarEstudiante(new Estudiante("", "2004", new ProgramaGrupal(), new MatriculaRegular()));
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

// caso 2
try
{
    ProgramaPersonalizado programaInvalido = new ProgramaPersonalizado(-2);
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

// caso 3
try
{
    academia.RegistrarEstudiante(new Estudiante("Mateo López", "2001", new ProgramaGrupal(), new MatriculaRegular()));
}
catch (EstudianteDuplicadoException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
