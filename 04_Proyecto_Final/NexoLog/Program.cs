using System.Globalization;
using System.Text;
using NexoLog;
using NexoLog.DTOs;
using NexoLog.Exportadores;
using NexoLog.Importadores;

Console.OutputEncoding = Encoding.UTF8;
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

Empresa sistema = new Empresa();
string carpetaDatos = LocalizarCarpetaDatos();
bool continuar = true;
bool archivosCargados = false;

while (continuar)
{
    Console.WriteLine("\n========================================================");
    Console.WriteLine("        SISTEMA DE LOGISTICA Y DISTRIBUCION - NEXOLOG   ");
    Console.WriteLine("========================================================");
    Console.WriteLine(" 1. Cargar archivos de datos");
    Console.WriteLine(" 2. Ver informe operativo");
    Console.WriteLine(" 3. Ver informe financiero");
    Console.WriteLine(" 4. Consultar informe de inconsistencias");
    Console.WriteLine(" 5. Exportar informes (JSON / XML)");
    Console.WriteLine(" 0. Salir");
    Console.WriteLine("--------------------------------------------------------");
    Console.Write(" Seleccione una opcion [0-5]: ");

    string lineaLeida = Console.ReadLine();
    if (lineaLeida == null)
    {
        break;
    }

    string opcion = lineaLeida.Trim().Trim('\uFEFF');
    if (string.IsNullOrEmpty(opcion))
    {
        continue;
    }
    Console.WriteLine();

    continuar = opcion switch
    {
        "1" => CargarArchivos(sistema, carpetaDatos),
        "2" => VerInformeOperativo(sistema),
        "3" => VerInformeFinanciero(sistema),
        "4" => VerInformeInconsistencias(sistema),
        "5" => ExportarInformes(sistema),
        "0" => Salir(),
        _ => MostrarOpcionInvalida()
    };
}

bool CargarArchivos(Empresa sys, string ruta)
{
    Console.WriteLine("--- Carga de Archivos ---");
    if (!Directory.Exists(ruta))
    {
        Console.WriteLine($"Error: no se encontro la carpeta de datos en '{ruta}'.");
        return true;
    }

    sys.Limpiar();
    List<RegistroInconsistencia> inconsistenciasLectura = new List<RegistroInconsistencia>();

    IImportadorArchivo<AcuerdoDto> importadorAcuerdos = ImportadorFactory.CrearAcuerdos("json");
    IImportadorArchivo<Despacho> importadorCaribe = ImportadorFactory.CrearPedidos("json");
    IImportadorArchivo<Despacho> importadorAndino = ImportadorFactory.CrearPedidos("xml");
    IImportadorArchivo<Acontecimiento> importadorNorte = ImportadorFactory.CrearSeguimientos("json");
    IImportadorArchivo<Acontecimiento> importadorAndina = ImportadorFactory.CrearSeguimientos("xml");

    List<AcuerdoDto> acuerdos = importadorAcuerdos.Importar(Path.Combine(ruta, "acuerdos.json"), inconsistenciasLectura);
    List<Despacho> pedidosCaribe = importadorCaribe.Importar(Path.Combine(ruta, "pedidos_caribe.json"), inconsistenciasLectura);
    List<Despacho> pedidosAndino = importadorAndino.Importar(Path.Combine(ruta, "pedidos_andino.xml"), inconsistenciasLectura);
    List<Acontecimiento> seguimientosNorte = importadorNorte.Importar(Path.Combine(ruta, "seguimiento_norte.json"), inconsistenciasLectura);
    List<Acontecimiento> seguimientosAndina = importadorAndina.Importar(Path.Combine(ruta, "seguimiento_andina.xml"), inconsistenciasLectura);

    foreach (RegistroInconsistencia inc in inconsistenciasLectura)
    {
        sys.RegistrarInconsistencia(inc);
    }

    foreach (AcuerdoDto dto in acuerdos)
    {
        sys.RegistrarEstablecimiento(dto.MapToDomain());
    }

    foreach (Despacho d in pedidosCaribe)
    {
        sys.RegistrarDespacho(d, "pedidos_caribe.json");
    }

    foreach (Despacho d in pedidosAndino)
    {
        sys.RegistrarDespacho(d, "pedidos_andino.xml");
    }

    sys.RegistrarSeguimientos(seguimientosNorte, "seguimiento_norte.json");
    sys.RegistrarSeguimientos(seguimientosAndina, "seguimiento_andina.xml");

    archivosCargados = true;

    Console.WriteLine($"Establecimientos registrados: {sys.Establecimientos.Count}");
    Console.WriteLine($"Despachos unicos registrados: {sys.Despachos.Count}");
    Console.WriteLine($"Inconsistencias detectadas: {sys.Inconsistencias.Count}");
    Console.WriteLine("Archivos cargados exitosamente.");
    return true;
}

bool VerInformeOperativo(Empresa sys)
{
    Console.WriteLine("--- Informe Operativo ---");
    if (!archivosCargados)
    {
        Console.WriteLine("Primero debe cargar los archivos (Opcion 1).");
        return true;
    }

    sys.MostrarInformeOperativo();
    return true;
}

bool VerInformeFinanciero(Empresa sys)
{
    Console.WriteLine("--- Informe Financiero ---");
    if (!archivosCargados)
    {
        Console.WriteLine("Primero debe cargar los archivos (Opcion 1).");
        return true;
    }

    sys.MostrarInformeFinanciero();
    return true;
}

bool VerInformeInconsistencias(Empresa sys)
{
    Console.WriteLine("--- Informe de Inconsistencias ---");
    if (!archivosCargados)
    {
        Console.WriteLine("Primero debe cargar los archivos (Opcion 1).");
        return true;
    }

    sys.MostrarInformeInconsistencias();
    return true;
}

bool ExportarInformes(Empresa sys)
{
    Console.WriteLine("--- Exportacion de Informes ---");
    if (!archivosCargados)
    {
        Console.WriteLine("Primero debe cargar los archivos (Opcion 1).");
        return true;
    }

    Console.WriteLine("1. Exportar a JSON");
    Console.WriteLine("2. Exportar a XML");
    Console.Write("Seleccione formato [1 o 2 / json o xml]: ");
    string f = (Console.ReadLine() ?? string.Empty).Trim();

    try
    {
        IExportadorInforme exportador = ExportadorFactory.Crear(f);
        string rutaDefecto = Path.Combine(Directory.GetCurrentDirectory(), "InformesExportados", $"informes_nexolog.{exportador.Extension}");
        Console.Write($"Ruta destino (ENTER para '{rutaDefecto}'): ");
        string rutaInput = (Console.ReadLine() ?? string.Empty).Trim();
        string rutaFinal = string.IsNullOrWhiteSpace(rutaInput) ? rutaDefecto : rutaInput;

        InformeConsolidadoDto dto = sys.MapToDto();
        exportador.Exportar(dto, rutaFinal);
        Console.WriteLine($"Informes exportados exitosamente en:\n  {Path.GetFullPath(rutaFinal)}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al exportar: {ex.Message}");
    }

    return true;
}

bool Salir()
{
    Console.WriteLine("Gracias por utilizar NexoLog. Fin del programa.");
    return false;
}

bool MostrarOpcionInvalida()
{
    Console.WriteLine("Opcion invalida. Intente nuevamente.");
    return true;
}

string LocalizarCarpetaDatos()
{
    string[] rutas = new string[]
    {
        Path.Combine(Directory.GetCurrentDirectory(), "DatosPrueba"),
        Path.Combine(Directory.GetCurrentDirectory(), "04_Proyecto_Final", "NexoLog", "DatosPrueba"),
        Path.Combine(AppContext.BaseDirectory, "DatosPrueba"),
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "DatosPrueba"),
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "DatosPrueba")
    };

    foreach (string r in rutas)
    {
        if (Directory.Exists(r) && File.Exists(Path.Combine(r, "acuerdos.json")))
        {
            return Path.GetFullPath(r);
        }
    }

    return Path.Combine(Directory.GetCurrentDirectory(), "DatosPrueba");
}
