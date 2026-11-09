using System.Globalization;
using System.Text;
using NexoLog;
using NexoLog.DTOs;
using NexoLog.Exportadores;
using NexoLog.Importadores;
using NexoLog.Informes;

Console.OutputEncoding = Encoding.UTF8;
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

Empresa sistema = new Empresa();
string carpetaDatos = LocalizarCarpetaDatos();
bool continuar = true;

List<AcuerdoDto> acuerdosCargados = new List<AcuerdoDto>();
List<Despacho> pedidosCaribeCargados = new List<Despacho>();
List<Despacho> pedidosAndinoCargados = new List<Despacho>();
List<Acontecimiento> seguimientosNorteCargados = new List<Acontecimiento>();
List<Acontecimiento> seguimientosAndinaCargados = new List<Acontecimiento>();
List<RegistroInconsistencia> inconsistenciasLectura = new List<RegistroInconsistencia>();

bool archivosCargados = false;
bool informacionIntegrada = false;

while (continuar)
{
    Console.WriteLine("\n========================================================");
    Console.WriteLine("        SISTEMA DE GESTION LOGISTICA - NEXOLOG          ");
    Console.WriteLine("========================================================");
    Console.WriteLine(" 1. Cargar archivos de datos");
    Console.WriteLine(" 2. Procesar e integrar informacion");
    Console.WriteLine(" 3. Ver informe operativo");
    Console.WriteLine(" 4. Ver informe financiero");
    Console.WriteLine(" 5. Ver informe de inconsistencias");
    Console.WriteLine(" 6. Exportar informes (JSON / XML)");
    Console.WriteLine(" 0. Salir");
    Console.WriteLine("--------------------------------------------------------");
    Console.Write(" Seleccione una opcion [0-6]: ");

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

    if (opcion == "1")
    {
        CargarArchivos(sistema, carpetaDatos);
    }
    else if (opcion == "2")
    {
        IntegrarInformacion(sistema);
    }
    else if (opcion == "3")
    {
        MostrarInformeOperativo(sistema);
    }
    else if (opcion == "4")
    {
        MostrarInformeFinanciero(sistema);
    }
    else if (opcion == "5")
    {
        MostrarInformeInconsistencias(sistema);
    }
    else if (opcion == "6")
    {
        ExportarInformes(sistema);
    }
    else if (opcion == "0")
    {
        Console.WriteLine("Gracias por utilizar NexoLog. Fin del programa.");
        continuar = false;
    }
    else
    {
        Console.WriteLine("Opcion invalida. Intente nuevamente.");
    }
}

void CargarArchivos(Empresa sys, string ruta)
{
    Console.WriteLine("--- Carga de Archivos ---");
    if (!Directory.Exists(ruta))
    {
        Console.WriteLine($"Error: no se encontro la carpeta de datos en '{ruta}'.");
        return;
    }

    acuerdosCargados.Clear();
    pedidosCaribeCargados.Clear();
    pedidosAndinoCargados.Clear();
    seguimientosNorteCargados.Clear();
    seguimientosAndinaCargados.Clear();
    inconsistenciasLectura.Clear();
    sys.Limpiar();

    string rutaAcuerdos = Path.Combine(ruta, "acuerdos.json");
    string rutaCaribe = Path.Combine(ruta, "pedidos_caribe.json");
    string rutaAndino = Path.Combine(ruta, "pedidos_andino.xml");
    string rutaNorte = Path.Combine(ruta, "seguimiento_norte.json");
    string rutaAndina = Path.Combine(ruta, "seguimiento_andina.xml");

    IImportadorArchivo<AcuerdoDto> importadorAcuerdos = CreadorImportadores.CrearImportadorAcuerdosJson();
    IImportadorArchivo<Despacho> importadorCaribe = CreadorImportadores.CrearImportadorPedidosJson();
    IImportadorArchivo<Despacho> importadorAndino = CreadorImportadores.CrearImportadorPedidosXml();
    IImportadorArchivo<Acontecimiento> importadorNorte = CreadorImportadores.CrearImportadorSeguimientosJson();
    IImportadorArchivo<Acontecimiento> importadorAndina = CreadorImportadores.CrearImportadorSeguimientosXml();

    acuerdosCargados = importadorAcuerdos.Importar(rutaAcuerdos, inconsistenciasLectura);
    pedidosCaribeCargados = importadorCaribe.Importar(rutaCaribe, inconsistenciasLectura);
    pedidosAndinoCargados = importadorAndino.Importar(rutaAndino, inconsistenciasLectura);
    seguimientosNorteCargados = importadorNorte.Importar(rutaNorte, inconsistenciasLectura);
    seguimientosAndinaCargados = importadorAndina.Importar(rutaAndina, inconsistenciasLectura);

    foreach (RegistroInconsistencia inc in inconsistenciasLectura)
    {
        sys.Inconsistencias.Add(inc);
    }

    archivosCargados = true;
    informacionIntegrada = false;

    int totalDespachosPrelim = pedidosCaribeCargados.Count + pedidosAndinoCargados.Count;
    int totalSeguimientosPrelim = seguimientosNorteCargados.Count + seguimientosAndinaCargados.Count;

    Console.WriteLine($"Acuerdos leidos: {acuerdosCargados.Count}");
    Console.WriteLine($"Despachos preliminares: {totalDespachosPrelim}");
    Console.WriteLine($"Hitos de seguimiento: {totalSeguimientosPrelim}");
    Console.WriteLine($"Inconsistencias detectadas en lectura: {inconsistenciasLectura.Count}");
    Console.WriteLine("Archivos cargados exitosamente.");
}

void IntegrarInformacion(Empresa sys)
{
    Console.WriteLine("--- Integracion de Informacion ---");
    if (!archivosCargados)
    {
        Console.WriteLine("Primero debe cargar los archivos (Opcion 1).");
        return;
    }

    sys.Limpiar();

    foreach (RegistroInconsistencia inc in inconsistenciasLectura)
    {
        sys.Inconsistencias.Add(inc);
    }

    foreach (AcuerdoDto dto in acuerdosCargados)
    {
        sys.RegistrarEstablecimiento(dto.MapToDomain());
    }

    foreach (Despacho d in pedidosCaribeCargados)
    {
        sys.RegistrarDespacho(d, "pedidos_caribe.json");
    }

    foreach (Despacho d in pedidosAndinoCargados)
    {
        sys.RegistrarDespacho(d, "pedidos_andino.xml");
    }

    sys.RegistrarSeguimientos(seguimientosNorteCargados, "seguimiento_norte.json");
    sys.RegistrarSeguimientos(seguimientosAndinaCargados, "seguimiento_andina.xml");

    informacionIntegrada = true;

    Console.WriteLine($"Establecimientos consolidados: {sys.Establecimientos.Count}");
    Console.WriteLine($"Despachos unicos integrados: {sys.Despachos.Count}");
    Console.WriteLine($"Inconsistencias totales registradas: {sys.Inconsistencias.Count}");
    Console.WriteLine("Integracion completada con exito.");
}

void MostrarInformeOperativo(Empresa sys)
{
    Console.WriteLine("--- Informe Operativo ---");
    if (!informacionIntegrada)
    {
        Console.WriteLine("Primero debe integrar la informacion (Opcion 2).");
        return;
    }

    InformeOperativo op = sys.GenerarInformeOperativo();
    Console.WriteLine($"Total despachos: {op.TotalDespachos}");
    Console.WriteLine($"Entregados: {op.TotalEntregados} | Devueltos: {op.TotalDevueltos} | En Distribucion: {op.TotalEnDistribucion} | Pendientes: {op.TotalPendientes}");
    Console.WriteLine($"Total paquetes: {op.TotalPaquetes} | Peso total: {op.PesoTotalKg:F2} kg");
    Console.WriteLine($"Cumplimiento SLA: {op.CumplidosSLA} cumplidos, {op.IncumplidosSLA} incumplidos");
    Console.WriteLine($"Porcentaje SLA sobre entregados: {op.PorcentajeCumplimientoSobreEntregados:F2}%");

    Console.WriteLine("\nDesglose por Comercio:");
    foreach (ResumenComercioOperativo com in op.DesgloseComercios)
    {
        Console.WriteLine($" - [{com.CodigoComercio}] {com.NombreComercio}: Total={com.TotalDespachos}, Entregados={com.Entregados}, Devueltos={com.Devueltos}, SLA={com.PorcentajeCumplimiento:F1}%");
    }

    Console.WriteLine("\nDesglose por Modalidad:");
    foreach (ResumenModalidadOperativo mod in op.DesgloseModalidades)
    {
        Console.WriteLine($" - {mod.Modalidad}: Total={mod.TotalDespachos}, Entregados={mod.Entregados}, Cumplen={mod.CumplidosSLA}, SLA={mod.PorcentajeCumplimiento:F1}%");
    }
}

void MostrarInformeFinanciero(Empresa sys)
{
    Console.WriteLine("--- Informe Financiero ---");
    if (!informacionIntegrada)
    {
        Console.WriteLine("Primero debe integrar la informacion (Opcion 2).");
        return;
    }

    InformeFinanciero fin = sys.GenerarInformeFinanciero();
    Console.WriteLine("Liquidacion por comercio:");
    foreach (ItemFinancieroComercio item in fin.DesgloseComercios)
    {
        Console.WriteLine($" - [{item.CodigoComercio}] {item.NombreComercio} ({item.ModalidadTarifa}): Facturables={item.DespachosFacturables}, Total=${item.TotalFacturado:N0}");
    }

    Console.WriteLine($"\nGran Total Facturado: ${fin.GranTotalFacturado:N0}");
    Console.WriteLine($"Despachos entregados no valorados: {fin.DespachosEntregadosNoValorados}");

    Despacho nx1001 = sys.ObtenerDespacho("NX-1001");
    if (nx1001 != null && nx1001.EsFacturable())
    {
        Establecimiento est = sys.ObtenerEstablecimiento("COM-01");
        if (est != null && est.TarifaAcordada != null)
        {
            decimal cobro = est.TarifaAcordada.CalcularTarifa(nx1001);
            Console.WriteLine($"\n[Prueba de control] Guia NX-1001: Peso={nx1001.CalcularPesoTotal():F1} kg | Cobro=${cobro:N0} (Esperado: $12.950)");
        }
    }
}

void MostrarInformeInconsistencias(Empresa sys)
{
    Console.WriteLine("--- Informe de Inconsistencias ---");
    InformeInconsistencias inc = sys.GenerarInformeInconsistencias();
    Console.WriteLine($"Total inconsistencias registradas: {inc.TotalInconsistencias}");

    Console.WriteLine("\nPor tipo:");
    foreach (KeyValuePair<string, int> par in inc.InconsistenciasPorTipo)
    {
        Console.WriteLine($" - {par.Key}: {par.Value}");
    }

    Console.WriteLine("\nPor archivo origen:");
    foreach (KeyValuePair<string, int> par in inc.InconsistenciasPorArchivo)
    {
        Console.WriteLine($" - {par.Key}: {par.Value}");
    }

    Console.WriteLine("\nDetalle de inconsistencias:");
    int i = 1;
    foreach (RegistroInconsistencia reg in inc.Listado)
    {
        Console.WriteLine($" {i}. [{reg.Tipo}] ({reg.ArchivoOrigen} | {reg.IdentificadorReferencia}): {reg.Motivo}");
        i++;
    }
}

void ExportarInformes(Empresa sys)
{
    Console.WriteLine("--- Exportacion de Informes ---");
    if (!informacionIntegrada)
    {
        Console.WriteLine("Primero debe integrar la informacion (Opcion 2).");
        return;
    }

    Console.WriteLine("1. Exportar a JSON");
    Console.WriteLine("2. Exportar a XML");
    Console.Write("Seleccione formato [1 o 2]: ");
    string f = (Console.ReadLine() ?? string.Empty).Trim();

    string extension = f == "2" ? "xml" : "json";
    IExportadorInforme exportador = f == "2" ? CreadorExportadores.CrearExportadorXml() : CreadorExportadores.CrearExportadorJson();

    string rutaDefecto = Path.Combine(Directory.GetCurrentDirectory(), "InformesExportados", $"informes_nexolog.{extension}");
    Console.Write($"Ruta destino (ENTER para '{rutaDefecto}'): ");
    string rutaInput = (Console.ReadLine() ?? string.Empty).Trim();
    string rutaFinal = string.IsNullOrWhiteSpace(rutaInput) ? rutaDefecto : rutaInput;

    InformeOperativo op = sys.GenerarInformeOperativo();
    InformeFinanciero fin = sys.GenerarInformeFinanciero();
    InformeInconsistencias inc = sys.GenerarInformeInconsistencias();

    try
    {
        exportador.Exportar(op, fin, inc, rutaFinal);
        Console.WriteLine($"Informes exportados exitosamente en:\n  {Path.GetFullPath(rutaFinal)}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al exportar: {ex.Message}");
    }
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
