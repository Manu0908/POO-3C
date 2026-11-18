using NexoLog.DTOs;

namespace NexoLog;

public class Acontecimiento
{
    public const string TipoRec = "REC";
    public const string TipoRut = "RUT";
    public const string TipoEnt = "ENT";
    public const string TipoDev = "DEV";

    private List<string> _acontecimientosExistentes = new List<string> { TipoRec, TipoRut, TipoEnt, TipoDev };
    private string _idAcontecimiento;
    private string _numeroGuia;
    private string _tipo;

    private Acontecimiento() { }

    public Acontecimiento(string idAcontecimiento, string numeroGuia, string tipo, DateTime fechaHora)
    {
        IdAcontecimiento = idAcontecimiento;
        NumeroGuia = numeroGuia;
        Tipo = tipo;
        FechaHora = fechaHora;
    }

    public DateTime FechaHora { get; private set; }

    public string IdAcontecimiento
    {
        get { return _idAcontecimiento; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NexoLogException("El identificador del acontecimiento es obligatorio.");
            }
            _idAcontecimiento = value.Trim();
        }
    }

    public string NumeroGuia
    {
        get { return _numeroGuia; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NexoLogException("El numero de guia asociado al acontecimiento es obligatorio.");
            }
            _numeroGuia = value.Trim();
        }
    }

    public string Tipo
    {
        get { return _tipo; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NexoLogException("El tipo de acontecimiento es obligatorio.");
            }

            string limpio = value.Trim().ToUpper();
            string normalizado = limpio switch
            {
                "INGRESO" => TipoRec,
                "RUTA" => TipoRut,
                "ENTREGA" => TipoEnt,
                "RETORNO" => TipoDev,
                _ => limpio
            };
            if (!_acontecimientosExistentes.Contains(normalizado))
            {
                throw new NexoLogException($"Tipo de acontecimiento desconocido: '{value}'.");
            }
            _tipo = normalizado;
        }
    }

    public static Acontecimiento MapToDomain(AcontecimientoDto dto)
    {
        if (dto == null)
        {
            throw new NexoLogException("El DTO del acontecimiento no puede ser nulo.");
        }

        return new Acontecimiento(dto.IdAcontecimiento, dto.NumeroGuia, dto.Tipo, dto.FechaHora);
    }

    public string MostrarInformacion()
    {
        return $"[{FechaHora:yyyy-MM-dd HH:mm:ss}] Guia: {NumeroGuia} | Tipo: {Tipo} | ID: {IdAcontecimiento}";
    }
}
