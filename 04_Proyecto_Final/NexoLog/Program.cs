using System.Globalization;
using System.Text;
using NexoLog.Dominio;
using NexoLog.Dominio.Inconsistencias;
using NexoLog.Infraestructura.Exportadores;
using NexoLog.Infraestructura.Lectores;
using NexoLog.Infraestructura.Lectores.DTOs;
using NexoLog.Servicios;
using NexoLog.Servicios.Modelos;

Console.OutputEncoding = Encoding.UTF8;
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

ServicioIntegracion servicioIntegracion = new ServicioIntegracion();
ServicioInformes servicioInformes = new ServicioInformes();

List<AcuerdoDto> acuerdosCargados = new List<AcuerdoDto>();
List<Despacho> pedidosCaribeCargados = new List<Despacho>();
List<Despacho> pedidosAndinoCargados = new List<Despacho>();
List<Acontecimiento> seguimientosNorteCargados = new List<Acontecimiento>();
List<Acontecimiento> seguimientosAndinaCargados = new List<Acontecimiento>();
List<RegistroInconsistencia> inconsistenciasLectura = new List<RegistroInconsistencia>();

bool archivosCargados = false;
bool informacionIntegrada = false;

InformeOperativo ultimoInformeOperativo = null;
InformeFinanciero ultimoInformeFinanciero = null;
InformeInconsistencias ultimoInformeInconsistencias = null;

string carpetaDatos = LocalizarCarpetaDatos();

// Soporte para ejecución en modo batch/demostración no interactivo
if (args.Length > 0 && (args[0] == "--demo" || args[0] == "--test"))
{
    EjecutarDemostracionAutomatica();
    return;
}

// Bucle principal del menú interactivo en consola
bool continuar = true;
while (continuar)
{
    MostrarEncabezadoMenu();
    Console.WriteLine(" 1. Cargar los archivos suministrados por NexoLog.");
    Console.WriteLine(" 2. Procesar e integrar la información de las diferentes fuentes.");
    Console.WriteLine(" 3. Generar y visualizar el informe operativo.");
    Console.WriteLine(" 4. Generar y visualizar el informe financiero.");
    Console.WriteLine(" 5. Consultar el informe de inconsistencias.");
    Console.WriteLine(" 6. Exportar los informes generados en JSON o XML.");
    Console.WriteLine(" 0. Salir.");
    Console.WriteLine("-------------------------------------------------------------------------------");
    Console.Write(" Seleccione una opción [0-6]: ");

    string entrada = Console.ReadLine();
    Console.WriteLine();

    switch (entrada != null ? entrada.Trim() : string.Empty)
    {
        case "1":
            OpcionCargarArchivos();
            break;

        case "2":
            OpcionProcesarIntegracion();
            break;

        case "3":
            OpcionVisualizarInformeOperativo();
            break;

        case "4":
            OpcionVisualizarInformeFinanciero();
            break;

        case "5":
            OpcionConsultarInconsistencias();
            break;

        case "6":
            OpcionExportarInformes();
            break;

        case "0":
            Console.WriteLine("¡Gracias por utilizar el Sistema de Gestión Logística NexoLog S.A.S.!");
            continuar = false;
            break;

        default:
            MostrarMensajeAlerta("Opción inválida. Ingrese un dígito del 0 al 6.");
            break;
    }

    if (continuar)
    {
        Console.WriteLine("\nPresione cualquier tecla para volver al menú principal...");
        Console.ReadKey(true);
        Console.Clear();
    }
}

// ============================================================================
// FUNCIONES DE CONTROL DE OPCIONES DEL MENÚ
// ============================================================================

void MostrarEncabezadoMenu()
{
    Console.WriteLine("===============================================================================");
    Console.WriteLine("                 NEXOLOG S.A.S. - SISTEMA INTEGRAL LOGÍSTICO                   ");
    Console.WriteLine("                 Consola de Gestión y Auditoría Operacional                    ");
    Console.WriteLine("===============================================================================");
    Console.WriteLine($" Estado actual: [Archivos cargados: {(archivosCargados ? "SÍ" : "NO")}] | [Integración: {(informacionIntegrada ? "SÍ" : "NO")}]");
    Console.WriteLine($" Directorio de datos: {carpetaDatos}");
    Console.WriteLine("===============================================================================");
}

void OpcionCargarArchivos()
{
    Console.WriteLine(">>> [OPCIÓN 1] CARGA DE ARCHIVOS FUENTE SUMINISTRADOS POR NEXOLOG");
    Console.WriteLine("-------------------------------------------------------------------------------");

    if (!Directory.Exists(carpetaDatos))
    {
        MostrarMensajeAlerta($"No se localizó la carpeta de datos de prueba en: {carpetaDatos}");
        return;
    }

    acuerdosCargados.Clear();
    pedidosCaribeCargados.Clear();
    pedidosAndinoCargados.Clear();
    seguimientosNorteCargados.Clear();
    seguimientosAndinaCargados.Clear();
    inconsistenciasLectura.Clear();

    string rutaAcuerdos = Path.Combine(carpetaDatos, "acuerdos.json");
    string rutaPedidosCaribe = Path.Combine(carpetaDatos, "pedidos_caribe.json");
    string rutaPedidosAndino = Path.Combine(carpetaDatos, "pedidos_andino.xml");
    string rutaSeguimientoNorte = Path.Combine(carpetaDatos, "seguimiento_norte.json");
    string rutaSeguimientoAndina = Path.Combine(carpetaDatos, "seguimiento_andina.xml");

    ILectorArchivo<AcuerdoDto> lectorAcuerdos = LectorArchivoFactory.CrearLectorAcuerdosJson();
    ILectorArchivo<Despacho> lectorPedidosJson = LectorArchivoFactory.CrearLectorPedidosJson();
    ILectorArchivo<Despacho> lectorPedidosXml = LectorArchivoFactory.CrearLectorPedidosXml();
    ILectorArchivo<Acontecimiento> lectorSeguimientoJson = LectorArchivoFactory.CrearLectorSeguimientosJson();
    ILectorArchivo<Acontecimiento> lectorSeguimientoXml = LectorArchivoFactory.CrearLectorSeguimientosXml();

    Console.WriteLine("Leyendo acuerdos comerciales (JSON)...");
    acuerdosCargados = lectorAcuerdos.Leer(rutaAcuerdos, inconsistenciasLectura);
    Console.WriteLine($"  -> {acuerdosCargados.Count} acuerdo(s) cargado(s) exitosamente.");

    Console.WriteLine("Leyendo pedidos del Caribe y Sierra (JSON)...");
    pedidosCaribeCargados = lectorPedidosJson.Leer(rutaPedidosCaribe, inconsistenciasLectura);
    Console.WriteLine($"  -> {pedidosCaribeCargados.Count} solicitud(es) válida(s) obtenida(s).");

    Console.WriteLine("Leyendo pedidos Andino (XML)...");
    pedidosAndinoCargados = lectorPedidosXml.Leer(rutaPedidosAndino, inconsistenciasLectura);
    Console.WriteLine($"  -> {pedidosAndinoCargados.Count} solicitud(es) válida(s) obtenida(s).");

    Console.WriteLine("Leyendo seguimientos Norte (JSON)...");
    seguimientosNorteCargados = lectorSeguimientoJson.Leer(rutaSeguimientoNorte, inconsistenciasLectura);
    Console.WriteLine($"  -> {seguimientosNorteCargados.Count} acontecimiento(s) obtenido(s).");

    Console.WriteLine("Leyendo seguimientos Andina (XML)...");
    seguimientosAndinaCargados = lectorSeguimientoXml.Leer(rutaSeguimientoAndina, inconsistenciasLectura);
    Console.WriteLine($"  -> {seguimientosAndinaCargados.Count} acontecimiento(s) obtenido(s).");

    archivosCargados = true;
    informacionIntegrada = false;

    Console.WriteLine("\n[RESUMEN DE CARGA]:");
    Console.WriteLine($" * Acuerdos en memoria: {acuerdosCargados.Count}");
    Console.WriteLine($" * Despachos preliminares: {pedidosCaribeCargados.Count + pedidosAndinoCargados.Count}");
    Console.WriteLine($" * Hitos de seguimiento: {seguimientosNorteCargados.Count + seguimientosAndinaCargados.Count}");
    Console.WriteLine($" * Inconsistencias de formato/datos detectadas en la lectura: {inconsistenciasLectura.Count}");
    Console.WriteLine("\n[OK] ¡Los archivos fueron leídos e interpretados de manera resiliente!");
}

void OpcionProcesarIntegracion()
{
    Console.WriteLine(">>> [OPCIÓN 2] PROCESAMIENTO E INTEGRACIÓN MULTIFUENTE");
    Console.WriteLine("-------------------------------------------------------------------------------");

    if (!archivosCargados)
    {
        MostrarMensajeAlerta("Debe cargar primero los archivos suministrados (Opción 1).");
        return;
    }

    servicioIntegracion.Reiniciar();

    // 1. Agregar las inconsistencias capturadas durante la lectura de archivos
    servicioIntegracion.AgregarInconsistencias(inconsistenciasLectura);

    // 2. Integrar acuerdos comerciales
    Console.WriteLine("1. Integrando acuerdos comerciales...");
    servicioIntegracion.IntegrarAcuerdos(acuerdosCargados, "acuerdos.json");

    // 3. Integrar lotes de despachos (resolución de duplicados y conflictos de guía)
    Console.WriteLine("2. Integrando solicitudes de despacho del Caribe (JSON)...");
    servicioIntegracion.IntegrarDespachos(pedidosCaribeCargados, "pedidos_caribe.json");

    Console.WriteLine("3. Integrando solicitudes de despacho Andino (XML)...");
    servicioIntegracion.IntegrarDespachos(pedidosAndinoCargados, "pedidos_andino.xml");

    // 4. Integrar acontecimientos de seguimiento (ordenamiento cronológico y máquina de estados)
    Console.WriteLine("4. Integrando y ordenando acontecimientos de Transportadora Norte (JSON)...");
    servicioIntegracion.IntegrarSeguimientos(seguimientosNorteCargados, "seguimiento_norte.json");

    Console.WriteLine("5. Integrando y ordenando acontecimientos de Transportadora Andina (XML)...");
    servicioIntegracion.IntegrarSeguimientos(seguimientosAndinaCargados, "seguimiento_andina.xml");

    // 5. Precompilar informes
    ultimoInformeOperativo = servicioInformes.GenerarInformeOperativo(servicioIntegracion.Despachos, servicioIntegracion.Establecimientos);
    ultimoInformeFinanciero = servicioInformes.GenerarInformeFinanciero(servicioIntegracion.Establecimientos, servicioIntegracion.Despachos);
    ultimoInformeInconsistencias = servicioInformes.GenerarInformeInconsistencias(servicioIntegracion.Inconsistencias);

    informacionIntegrada = true;

    Console.WriteLine("\n[RESULTADO DE LA INTEGRACIÓN]:");
    Console.WriteLine($" * Comercios/Establecimientos integrados: {servicioIntegracion.Establecimientos.Count}");
    Console.WriteLine($" * Despachos únicos consolidados: {servicioIntegracion.Despachos.Count}");
    Console.WriteLine($" * Total inconsistencias consolidadas en auditoría: {servicioIntegracion.Inconsistencias.Count}");
    Console.WriteLine("\n[OK] ¡Información procesada y consolidada exitosamente!");
}

void OpcionVisualizarInformeOperativo()
{
    Console.WriteLine(">>> [OPCIÓN 3] INFORME OPERATIVO CONSOLIDADO");
    Console.WriteLine("-------------------------------------------------------------------------------");

    if (!informacionIntegrada)
    {
        MostrarMensajeAlerta("Debe procesar e integrar la información primero (Opción 2).");
        return;
    }

    if (ultimoInformeOperativo == null)
    {
        ultimoInformeOperativo = servicioInformes.GenerarInformeOperativo(servicioIntegracion.Despachos, servicioIntegracion.Establecimientos);
    }

    Console.WriteLine("===============================================================================");
    Console.WriteLine("                     MÉTRICAS OPERATIVAS GLOBALES                              ");
    Console.WriteLine("===============================================================================");
    Console.WriteLine($" Total Despachos Gestionados:   {ultimoInformeOperativo.TotalDespachos,6}");
    Console.WriteLine($" Despachos Entregados:          {ultimoInformeOperativo.TotalEntregados,6}");
    Console.WriteLine($" Despachos Devueltos:           {ultimoInformeOperativo.TotalDevueltos,6}");
    Console.WriteLine($" Despachos En Distribución:     {ultimoInformeOperativo.TotalEnDistribucion,6}");
    Console.WriteLine($" Despachos Pendientes:          {ultimoInformeOperativo.TotalPendientes,6}");
    Console.WriteLine($" Total Paquetes Transportados:  {ultimoInformeOperativo.TotalPaquetes,6}");
    Console.WriteLine($" Peso Total Transportado:       {ultimoInformeOperativo.PesoTotalKg,10:F2} kg");
    Console.WriteLine("-------------------------------------------------------------------------------");
    Console.WriteLine($" Cumplimientos de SLA:          {ultimoInformeOperativo.CumplidosSLA,6}");
    Console.WriteLine($" Incumplimientos de SLA:        {ultimoInformeOperativo.IncumplidosSLA,6}");
    Console.WriteLine($" % Cumplimiento sobre Entregados:{ultimoInformeOperativo.PorcentajeCumplimientoSobreEntregados,9:F2}%");
    Console.WriteLine("===============================================================================");

    Console.WriteLine("\n--- DESGLOSE OPERATIVO POR COMERCIO REMITENTE ---");
    Console.WriteLine(string.Format("{0,-8} | {1,-20} | {2,5} | {3,5} | {4,5} | {5,5} | {6,8}",
        "Código", "Nombre Comercio", "Total", "Entr.", "Dev.", "Cumple", "% Cumpl."));
    Console.WriteLine(new string('-', 79));

    foreach (ResumenComercioOperativo com in ultimoInformeOperativo.DesgloseComercios)
    {
        Console.WriteLine(string.Format("{0,-8} | {1,-20} | {2,5} | {3,5} | {4,5} | {5,5} | {6,7:F1}%",
            com.CodigoComercio,
            com.NombreComercio.Length > 20 ? com.NombreComercio.Substring(0, 17) + "..." : com.NombreComercio,
            com.TotalDespachos,
            com.Entregados,
            com.Devueltos,
            com.CumplidosSLA,
            com.PorcentajeCumplimiento));
    }

    Console.WriteLine("\n--- DESGLOSE OPERATIVO POR MODALIDAD DE SERVICIO (SLA) ---");
    Console.WriteLine(string.Format("{0,-15} | {1,8} | {2,10} | {3,8} | {4,9} | {5,9}",
        "Modalidad", "Total", "Entregados", "Cumplen", "Incumplen", "% Cumpl."));
    Console.WriteLine(new string('-', 71));

    foreach (ResumenModalidadOperativo mod in ultimoInformeOperativo.DesgloseModalidades)
    {
        Console.WriteLine(string.Format("{0,-15} | {1,8} | {2,10} | {3,8} | {4,9} | {5,8:F1}%",
            mod.Modalidad,
            mod.TotalDespachos,
            mod.Entregados,
            mod.CumplidosSLA,
            mod.IncumplidosSLA,
            mod.PorcentajeCumplimiento));
    }
}

void OpcionVisualizarInformeFinanciero()
{
    Console.WriteLine(">>> [OPCIÓN 4] INFORME FINANCIERO Y LIQUIDACIÓN");
    Console.WriteLine("-------------------------------------------------------------------------------");

    if (!informacionIntegrada)
    {
        MostrarMensajeAlerta("Debe procesar e integrar la información primero (Opción 2).");
        return;
    }

    if (ultimoInformeFinanciero == null)
    {
        ultimoInformeFinanciero = servicioInformes.GenerarInformeFinanciero(servicioIntegracion.Establecimientos, servicioIntegracion.Despachos);
    }

    Console.WriteLine("===============================================================================");
    Console.WriteLine("                     LIQUIDACIÓN FINANCIERA POR COMERCIO                       ");
    Console.WriteLine("===============================================================================");
    Console.WriteLine(string.Format("{0,-8} | {1,-18} | {2,-17} | {3,10} | {4,5} | {5,12}",
        "Código", "Comercio", "Modalidad Tarifa", "TarifaBase", "Fact.", "Total Cobro"));
    Console.WriteLine(new string('-', 82));

    foreach (ItemFinancieroComercio item in ultimoInformeFinanciero.DesgloseComercios)
    {
        Console.WriteLine(string.Format("{0,-8} | {1,-18} | {2,-17} | ${3,9:N0} | {4,5} | ${5,11:N0}",
            item.CodigoComercio,
            item.NombreComercio.Length > 18 ? item.NombreComercio.Substring(0, 15) + "..." : item.NombreComercio,
            item.ModalidadTarifa,
            item.TarifaBase,
            item.DespachosFacturables,
            item.TotalFacturado));
    }

    Console.WriteLine("-------------------------------------------------------------------------------");
    Console.WriteLine("\n--- RESUMEN POR MODALIDAD DE TARIFA ---");
    foreach (KeyValuePair<string, decimal> par in ultimoInformeFinanciero.CobrosPorModalidadTarifa)
    {
        Console.WriteLine($" * {par.Key,-25}: ${par.Value,14:N0}");
    }

    Console.WriteLine(new string('=', 82));
    Console.WriteLine($" GRAN TOTAL FACTURADO:                           ${ultimoInformeFinanciero.GranTotalFacturado,14:N0}");
    Console.WriteLine($" Despachos entregados no valorados:               {ultimoInformeFinanciero.DespachosEntregadosNoValorados,6}");
    Console.WriteLine(new string('=', 82));

    // Verificación especial requerida en enunciado: NX-1001
    Despacho nx1001 = servicioIntegracion.Despachos.FirstOrDefault(d => d.NumeroGuia == "NX-1001");
    if (nx1001 != null && nx1001.EsFacturable())
    {
        Establecimiento estCaribe = servicioIntegracion.Establecimientos.FirstOrDefault(e => e.Codigo == "COM-01");
        if (estCaribe != null && estCaribe.TarifaAcordada != null)
        {
            decimal cobroNx1001 = estCaribe.TarifaAcordada.CalcularTarifa(nx1001);
            Console.WriteLine($"\n[COMPROBACIÓN ENUNCIADO] Despacho NX-1001:");
            Console.WriteLine($" Peso: {nx1001.CalcularPesoTotal():F2} kg | Tarifa base: $3.500/kg | Liquidado: ${cobroNx1001:N0} (Esperado: $12.950)");
        }
    }
}

void OpcionConsultarInconsistencias()
{
    Console.WriteLine(">>> [OPCIÓN 5] INFORME DE AUDITORÍA E INCONSISTENCIAS");
    Console.WriteLine("-------------------------------------------------------------------------------");

    if (ultimoInformeInconsistencias == null)
    {
        ultimoInformeInconsistencias = servicioInformes.GenerarInformeInconsistencias(servicioIntegracion.Inconsistencias);
    }

    Console.WriteLine($"Total de inconsistencias registradas: {ultimoInformeInconsistencias.TotalInconsistencias}");
    Console.WriteLine("-------------------------------------------------------------------------------");

    Console.WriteLine("\n--- AGRUPADAS POR TIPO DE INCONSISTENCIA ---");
    foreach (KeyValuePair<TipoInconsistencia, int> par in ultimoInformeInconsistencias.InconsistenciasPorTipo)
    {
        Console.WriteLine($" * {par.Key,-28}: {par.Value,4} ocurrencia(s)");
    }

    Console.WriteLine("\n--- AGRUPADAS POR ARCHIVO ORIGEN ---");
    foreach (KeyValuePair<string, int> par in ultimoInformeInconsistencias.InconsistenciasPorArchivo)
    {
        Console.WriteLine($" * {par.Key,-28}: {par.Value,4} ocurrencia(s)");
    }

    Console.WriteLine("\n--- LISTADO DETALLADO DE INCONSISTENCIAS ---");
    Console.WriteLine(string.Format("{0,-4} | {1,-22} | {2,-15} | {3,-26} | {4}",
        "N°", "Archivo", "Referencia", "Tipo", "Motivo"));
    Console.WriteLine(new string('-', 100));

    int indice = 1;
    foreach (RegistroInconsistencia inc in ultimoInformeInconsistencias.Listado)
    {
        string motivoTruncado = inc.Motivo.Length > 45 ? inc.Motivo.Substring(0, 42) + "..." : inc.Motivo;
        Console.WriteLine(string.Format("{0,4} | {1,-22} | {2,-15} | {3,-26} | {4}",
            indice,
            inc.ArchivoOrigen.Length > 22 ? inc.ArchivoOrigen.Substring(0, 19) + "..." : inc.ArchivoOrigen,
            inc.IdentificadorReferencia.Length > 15 ? inc.IdentificadorReferencia.Substring(0, 12) + "..." : inc.IdentificadorReferencia,
            inc.Tipo,
            motivoTruncado));
        indice++;
    }
}

void OpcionExportarInformes()
{
    Console.WriteLine(">>> [OPCIÓN 6] EXPORTAR INFORMES CONSOLIDADOS (JSON / XML)");
    Console.WriteLine("-------------------------------------------------------------------------------");

    if (!informacionIntegrada)
    {
        MostrarMensajeAlerta("Debe procesar e integrar la información primero (Opción 2).");
        return;
    }

    if (ultimoInformeOperativo == null)
    {
        ultimoInformeOperativo = servicioInformes.GenerarInformeOperativo(servicioIntegracion.Despachos, servicioIntegracion.Establecimientos);
    }
    if (ultimoInformeFinanciero == null)
    {
        ultimoInformeFinanciero = servicioInformes.GenerarInformeFinanciero(servicioIntegracion.Establecimientos, servicioIntegracion.Despachos);
    }
    if (ultimoInformeInconsistencias == null)
    {
        ultimoInformeInconsistencias = servicioInformes.GenerarInformeInconsistencias(servicioIntegracion.Inconsistencias);
    }

    Console.WriteLine("Formatos disponibles para exportación:");
    Console.WriteLine(" 1. Formato JSON (estructurado, indentado, UTF-8)");
    Console.WriteLine(" 2. Formato XML (estructurado, legible, UTF-8)");
    Console.Write("Seleccione formato [1 o 2]: ");

    string formato = Console.ReadLine();
    string formatoLimpio = formato != null ? formato.Trim() : string.Empty;
    string extension;
    IExportadorInforme exportador;

    if (formatoLimpio == "1")
    {
        extension = "json";
        exportador = new ExportadorJson();
    }
    else if (formatoLimpio == "2")
    {
        extension = "xml";
        exportador = new ExportadorXml();
    }
    else
    {
        MostrarMensajeAlerta("Formato no válido.");
        return;
    }

    string rutaSugerida = Path.Combine(Directory.GetCurrentDirectory(), "InformesExportados", $"informes_nexolog.{extension}");
    Console.Write($"Ruta destino (Presione ENTER para usar '{rutaSugerida}'): ");
    string rutaIngresada = Console.ReadLine();
    string rutaIngresadaLimpia = rutaIngresada != null ? rutaIngresada.Trim() : string.Empty;
    string rutaFinal = string.IsNullOrWhiteSpace(rutaIngresadaLimpia) ? rutaSugerida : rutaIngresadaLimpia;

    try
    {
        exportador.Exportar(ultimoInformeOperativo, ultimoInformeFinanciero, ultimoInformeInconsistencias, rutaFinal);
        Console.WriteLine($"\n[ÉXITO] Informes exportados exitosamente en:\n  -> {Path.GetFullPath(rutaFinal)}");
    }
    catch (Exception ex)
    {
        MostrarMensajeAlerta($"Error al exportar informes: {ex.Message}");
    }
}

void MostrarMensajeAlerta(string mensaje)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"[AVISO]: {mensaje}");
    Console.ResetColor();
}

string LocalizarCarpetaDatos()
{
    string[] posiblesRutas = new string[]
    {
        Path.Combine(Directory.GetCurrentDirectory(), "DatosPrueba"),
        Path.Combine(Directory.GetCurrentDirectory(), "04_Proyecto_Final", "NexoLog", "DatosPrueba"),
        Path.Combine(AppContext.BaseDirectory, "DatosPrueba"),
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "DatosPrueba"),
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "DatosPrueba")
    };

    foreach (string ruta in posiblesRutas)
    {
        if (Directory.Exists(ruta) && File.Exists(Path.Combine(ruta, "acuerdos.json")))
        {
            return Path.GetFullPath(ruta);
        }
    }

    return Path.Combine(Directory.GetCurrentDirectory(), "DatosPrueba");
}

void EjecutarDemostracionAutomatica()
{
    Console.WriteLine("===============================================================================");
    Console.WriteLine("         NEXOLOG S.A.S. - MODO DEMOSTRACIÓN AUTOMÁTICA (FASES 4, 5 Y 6)        ");
    Console.WriteLine("===============================================================================\n");

    OpcionCargarArchivos();
    Console.WriteLine();
    OpcionProcesarIntegracion();
    Console.WriteLine();
    OpcionVisualizarInformeOperativo();
    Console.WriteLine();
    OpcionVisualizarInformeFinanciero();
    Console.WriteLine();
    OpcionConsultarInconsistencias();
    Console.WriteLine();

    // Exportar automáticamente ambos formatos para comprobación completa
    Console.WriteLine(">>> EXPORTACIÓN AUTOMÁTICA DE INFORMES (JSON y XML)");
    string carpetaExport = Path.Combine(Directory.GetCurrentDirectory(), "InformesExportados");
    new ExportadorJson().Exportar(ultimoInformeOperativo, ultimoInformeFinanciero, ultimoInformeInconsistencias, Path.Combine(carpetaExport, "informes_nexolog.json"));
    new ExportadorXml().Exportar(ultimoInformeOperativo, ultimoInformeFinanciero, ultimoInformeInconsistencias, Path.Combine(carpetaExport, "informes_nexolog.xml"));
    Console.WriteLine($"[OK] Informes JSON y XML exportados en: {carpetaExport}");

    Console.WriteLine("\n===============================================================================");
    Console.WriteLine("              ¡DEMOSTRACIÓN AUTOMÁTICA COMPLETADA CON ÉXITO!                   ");
    Console.WriteLine("===============================================================================");
}
