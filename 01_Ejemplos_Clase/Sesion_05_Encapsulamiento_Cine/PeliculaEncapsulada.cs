namespace prueba;

public class PeliculaEncapsulada
{
    public static int DuracionLarga = 120;

    private string[] GenerosPermitidos =
        { "Acción", "Comedia", "Drama", "Terror", "Animación" };

    private string _codigo;
    private string _titulo;
    private string _genero;
    private int _duracionMinutos;
    private int _vecesReproducida;


    public string Codigo
    {
        get { return _codigo; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new Exception("El código es obligatorio y no puede estar vacío.");
            }
            if (value.Trim().Length < 3)
            {
                throw new Exception("El código debe tener mínimo 3 caracteres.");
            }
            _codigo = value.Trim();
        }
    }

    public string Titulo
    {
        get { return _titulo; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new Exception("El título es obligatorio.");
            }
            if (value.Trim().Length < 2)
            {
                throw new Exception("El título debe tener mínimo 2 caracteres.");
            }
            _titulo = value.Trim();
        }
    }

    public string Genero
    {
        get { return _genero; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new Exception("El género es obligatorio.");
            }
            if (!GenerosPermitidos.Contains(value.Trim()))
            {
                throw new Exception(
                    $"El género '{value}' no es válido. Valores permitidos: {string.Join(", ", GenerosPermitidos)}.");
            }
            _genero = value.Trim();
        }
    }

    public int DuracionMinutos
    {
        get { return _duracionMinutos; }
        private set
        {
            if (value <= 0)
            {
                throw new Exception("La duración debe ser mayor que 0.");
            }
            if (value > 300)
            {
                throw new Exception("La duración no puede superar los 300 minutos.");
            }
            _duracionMinutos = value;
        }
    }

    public int Reproducciones
    {
        get { return _vecesReproducida; }
    }

    public bool EsLarga => _duracionMinutos > DuracionLarga;

    private PeliculaEncapsulada() { }

    public PeliculaEncapsulada(string codigo, string titulo, string genero, int duracionMinutos)
    {
        Codigo = codigo;
        Titulo = titulo;
        Genero = genero;
        DuracionMinutos = duracionMinutos;
        _vecesReproducida = 0;
    }

    public void RegistrarReproduccion()
    {
        _vecesReproducida++;
    }
}
