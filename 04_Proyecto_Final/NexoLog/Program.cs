using System.Globalization;
using System.Text;
using NexoLog;
using NexoLog.DTOs;
using NexoLog.Exportadores;
using NexoLog.Importadores;

Console.OutputEncoding = Encoding.UTF8;
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

var sistema = new Empresa();
string carpetaDatos = LocalizarCarpetaDatos();
bool continuar = true;
bool archivosCargados = false;
bool datosConsolidados = false;

var inconsistenciasLectura = new List<RegistroInconsistencia>();
var acuerdosCargados = new List<AcuerdoDto>();
var pedidosCaribe = new List<Despacho>();
var pedidosAndino = new List<Despacho>();
var seguimientosNorte = new List<Acontecimiento>();
var seguimientosAndina = new List<Acontecimiento>();

while (continuar)
{
    Console.WriteLine("\n========================================================");
    Console.WriteLine("        SISTEMA DE LOGISTICA Y DISTRIBUCION - NEXOLOG   ");
    Console.WriteLine("========================================================");
    Console.WriteLine(" 1. Cargar archivos de datos");
    Console.WriteLine(" 2. Consolidar informacion de las fuentes");
    Console.WriteLine(" 3. Ver informe operativo");
    Console.WriteLine(" 4. Ver informe financiero");
    Console.WriteLine(" 5. Consultar informe de inconsistencias");
    Console.WriteLine(" 6. Exportar informes (JSON / XML)");
    Console.WriteLine(" 0. Salir");
    Console.WriteLine("--------------------------------------------------------");
    Console.Write(" Seleccione una opcion [0-6]: ");

    string lineaLeida = Console.ReadLine();
    if (lineaLeida == null)
    {
        break;
    }

    string opcion = lineaLeida.Trim();
    if (string.IsNullOrEmpty(opcion))
    {
        continue;
    }
    Console.WriteLine();

    continuar = opcion switch
    {
        "1" => CargarArchivos(carpetaDatos),
        "2" => ConsolidarFuentes(sistema),
        "3" => VerInformeOperativo(sistema),
        "4" => VerInformeFinanciero(sistema),
        "5" => VerInformeInconsistencias(sistema),
        "6" => ExportarInformes(sistema),
        "0" => Salir(),
        _ => MostrarOpcionInvalida()
    };
}

bool CargarArchivos(string ruta)
{
    Console.WriteLine("--- Carga de Archivos ---");
    if (!Directory.Exists(ruta))
    {
        Console.WriteLine($"Error: no se encontro la carpeta de datos en '{ruta}'.");
        return true;
    }

    inconsistenciasLectura = new List<RegistroInconsistencia>();

    var importadorAcuerdos = ImportadorFactory.CrearAcuerdos("json");
    var importadorCaribe = ImportadorFactory.CrearPedidos("json");
    var importadorAndino = ImportadorFactory.CrearPedidos("xml");
    var importadorNorte = ImportadorFactory.CrearSeguimientos("json");
    var importadorAndina = ImportadorFactory.CrearSeguimientos("xml");

    acuerdosCargados = importadorAcuerdos.Importar(Path.Combine(ruta, "acuerdos.json"), inconsistenciasLectura);
    pedidosCaribe = importadorCaribe.Importar(Path.Combine(ruta, "pedidos_caribe.json"), inconsistenciasLectura);
    pedidosAndino = importadorAndino.Importar(Path.Combine(ruta, "pedidos_andino.xml"), inconsistenciasLectura);
    seguimientosNorte = importadorNorte.Importar(Path.Combine(ruta, "seguimiento_norte.json"), inconsistenciasLectura);
    seguimientosAndina = importadorAndina.Importar(Path.Combine(ruta, "seguimiento_andina.xml"), inconsistenciasLectura);

    archivosCargados = true;
    datosConsolidados = false;

    Console.WriteLine($"Acuerdos leidos: {acuerdosCargados.Count}");
    Console.WriteLine($"Pedidos Caribe (JSON) leidos: {pedidosCaribe.Count}");
    Console.WriteLine($"Pedidos Andino (XML) leidos: {pedidosAndino.Count}");
    Console.WriteLine($"Seguimientos Norte (JSON) leidos: {seguimientosNorte.Count}");
    Console.WriteLine($"Seguimientos Andina (XML) leidos: {seguimientosAndina.Count}");
    Console.WriteLine($"Inconsistencias en lectura: {inconsistenciasLectura.Count}");
    Console.WriteLine("Archivos cargados. Continue con la Opcion 2 para consolidar las fuentes.");
    return true;
}

bool ConsolidarFuentes(Empresa sys)
{
    Console.WriteLine("--- Consolidacion de Fuentes ---");
    if (!archivosCargados)
    {
        Console.WriteLine("Primero debe cargar los archivos (Opcion 1).");
        return true;
    }

    if (datosConsolidados)
    {
        Console.WriteLine("Las fuentes ya fueron consolidadas. Cargue los archivos (Opcion 1) si desea reiniciar.");
        return true;
    }

    sys.Limpiar();

    foreach (var inc in inconsistenciasLectura)
    {
        sys.RegistrarInconsistencia(inc);
    }

    foreach (var dto in acuerdosCargados)
    {
        sys.RegistrarEstablecimiento(Establecimiento.MapToDomain(dto));
    }

    foreach (var d in pedidosCaribe)
    {
        sys.RegistrarDespacho(d, "pedidos_caribe.json");
    }

    foreach (var d in pedidosAndino)
    {
        sys.RegistrarDespacho(d, "pedidos_andino.xml");
    }

    sys.RegistrarSeguimientos(seguimientosNorte, "seguimiento_norte.json");
    sys.RegistrarSeguimientos(seguimientosAndina, "seguimiento_andina.xml");

    datosConsolidados = true;

    Console.WriteLine($"Establecimientos registrados: {sys.Establecimientos.Count}");
    Console.WriteLine($"Despachos unicos registrados: {sys.Despachos.Count}");
    Console.WriteLine($"Inconsistencias totales detectadas: {sys.Inconsistencias.Count}");
    Console.WriteLine("Informacion consolidada exitosamente.");
    return true;
}

bool VerInformeOperativo(Empresa sys)
{
    Console.WriteLine("--- Informe Operativo ---");
    if (!datosConsolidados)
    {
        Console.WriteLine("Primero debe cargar los archivos (Opcion 1) y consolidar las fuentes (Opcion 2).");
        return true;
    }

    sys.MostrarInformeOperativo();
    return true;
}

bool VerInformeFinanciero(Empresa sys)
{
    Console.WriteLine("--- Informe Financiero ---");
    if (!datosConsolidados)
    {
        Console.WriteLine("Primero debe cargar los archivos (Opcion 1) y consolidar las fuentes (Opcion 2).");
        return true;
    }

    sys.MostrarInformeFinanciero();
    return true;
}

bool VerInformeInconsistencias(Empresa sys)
{
    Console.WriteLine("--- Informe de Inconsistencias ---");
    if (!datosConsolidados)
    {
        Console.WriteLine("Primero debe cargar los archivos (Opcion 1) y consolidar las fuentes (Opcion 2).");
        return true;
    }

    sys.MostrarInformeInconsistencias();
    return true;
}

bool ExportarInformes(Empresa sys)
{
    Console.WriteLine("--- Exportacion de Informes ---");
    if (!datosConsolidados)
    {
        Console.WriteLine("Primero debe cargar los archivos (Opcion 1) y consolidar las fuentes (Opcion 2).");
        return true;
    }

    Console.Write("Seleccione formato (json / xml): ");
    string f = Console.ReadLine();
    if (f == null)
    {
        return true;
    }

    try
    {
        var exportador = ExportadorFactory.Crear(f.Trim());
        var dto = sys.MapToDto();
        exportador.Exportar(dto);
        Console.WriteLine($"Informes exportados exitosamente en: informes_nexolog.{exportador.TipoArchivo}");
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
    if (Directory.Exists("DatosPrueba"))
    {
        return "DatosPrueba";
    }

    string juntoAlEjecutable = Path.Combine(AppContext.BaseDirectory, "DatosPrueba");
    if (Directory.Exists(juntoAlEjecutable))
    {
        return juntoAlEjecutable;
    }

    return Path.Combine("04_Proyecto_Final", "NexoLog", "DatosPrueba");
}
