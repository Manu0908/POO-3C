namespace prueba;

public class Pelicula
{
    public static int DuracionLarga = 120;

    public string Codigo { get; private set; }
    public string Titulo { get; private set; }
    public string Genero { get; private set; }
    public int DuracionMinutos { get; private set; }
    public int VecesReproducida { get; private set; }
    public int Reproducciones { get { return VecesReproducida; } }
    public bool EsLarga => DuracionMinutos > DuracionLarga;

    private Pelicula() { }

    public Pelicula(string codigo, string titulo, string genero, int duracionMinutos)
    {
        Codigo = codigo;
        Titulo = titulo;
        Genero = genero;
        DuracionMinutos = duracionMinutos;
        VecesReproducida = 0;
    }

    public void RegistrarReproduccion()
    {
        VecesReproducida++;
    }
}
