using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using NexoLog;
using NexoLog.DTOs;
using NexoLog.Informes;
using NexoLog.Servicios;
using NexoLog.Tarifas;

namespace NexoLog.Tests;

public class Program
{
    private static int _passedCount = 0;
    private static int _failedCount = 0;
    private static List<string> _failures = new List<string>();

    public static int Main(string[] args)
    {
        Console.WriteLine("===============================================================");
        Console.WriteLine("   NEXOLOG S.A.S. - EMPIRICAL CHALLENGER TEST SUITE (M1)       ");
        Console.WriteLine("===============================================================");

        TestChallenge1_PrivateConstructors();
        TestChallenge2_ZeroReadonlyKeyword();
        TestChallenge3_EmpresaStructuralIsolation();
        TestChallenge4_MapToDomainRobustness();
        TestExtra_DespachoEsIdenticoA();
        TestExtra_InformeCrearEncapsulation();

        Console.WriteLine("\n===============================================================");
        Console.WriteLine($"RESULTS: {_passedCount} PASSED, {_failedCount} FAILED");
        Console.WriteLine("===============================================================");

        if (_failedCount > 0)
        {
            Console.WriteLine("\nFAILURES SUMMARY:");
            foreach (string fail in _failures)
            {
                Console.WriteLine($" [FAIL] {fail}");
            }
            return 1;
        }

        Console.WriteLine("\nALL TESTS PASSED EMPIRICALLY! VERDICT: APPROVE");
        return 0;
    }

    private static void Assert(bool condition, string testName, string details = "")
    {
        if (condition)
        {
            _passedCount++;
            Console.WriteLine($" [PASS] {testName}");
        }
        else
        {
            _failedCount++;
            string msg = $"{testName}: {details}";
            _failures.Add(msg);
            Console.WriteLine($" [FAIL] {msg}");
        }
    }

    private static void TestChallenge1_PrivateConstructors()
    {
        Console.WriteLine("\n--- CHALLENGE 1: Parameterless Private Constructors (R2) ---");

        Type[] domainTypes = new Type[]
        {
            typeof(Despacho),
            typeof(Paquete),
            typeof(Establecimiento),
            typeof(Acontecimiento),
            typeof(RegistroInconsistencia)
        };

        foreach (Type type in domainTypes)
        {
            // 1. Must NOT have a public parameterless constructor
            ConstructorInfo publicCtor = type.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public,
                null,
                Type.EmptyTypes,
                null);
            Assert(publicCtor == null, $"{type.Name} has NO public parameterless ctor");

            // 2. Must have a private parameterless constructor
            ConstructorInfo privateCtor = type.GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                Type.EmptyTypes,
                null);
            Assert(privateCtor != null, $"{type.Name} HAS non-public parameterless ctor");
            Assert(privateCtor != null && privateCtor.IsPrivate, $"{type.Name} parameterless ctor is strictly PRIVATE");

            // 3. Can be instantiated via reflection without throwing
            object instance = null;
            try
            {
                instance = privateCtor?.Invoke(null);
            }
            catch (Exception ex)
            {
                Assert(false, $"{type.Name} reflection instantiation succeeded", ex.Message);
            }
            Assert(instance != null, $"{type.Name} instantiated via reflection");

            // 4. Verify fields are non-null defaults
            if (instance is Despacho d)
            {
                Assert(d.NumeroGuia != null, "Despacho.NumeroGuia != null");
                Assert(d.CodigoEstablecimiento != null, "Despacho.CodigoEstablecimiento != null");
                Assert(d.Destinatario != null, "Despacho.Destinatario != null");
                Assert(d.CiudadDestino != null, "Despacho.CiudadDestino != null");
                Assert(d.Transportadora != null, "Despacho.Transportadora != null");
                Assert(d.EstadoActual == Despacho.EstadoPendiente, "Despacho.EstadoActual == EstadoPendiente");
                Assert(d.Paquetes != null && d.Paquetes.Count == 0, "Despacho.Paquetes initialized and empty");
                Assert(d.Historial != null && d.Historial.Count == 0, "Despacho.Historial initialized and empty");
                Assert(d.CantidadPaquetes == 0, "Despacho.CantidadPaquetes == 0");
                Assert(d.CalcularPesoTotal() == 0.0, "Despacho.CalcularPesoTotal() == 0.0");
                Assert(!d.EsFacturable(), "Despacho.EsFacturable() == false");
                Assert(!d.EvaluarCumplimiento(), "Despacho.EvaluarCumplimiento() == false");
                
                string info = d.MostrarInformacion();
                Assert(!string.IsNullOrEmpty(info), "Despacho.MostrarInformacion() returns valid string without throwing");
            }
            else if (instance is Paquete p)
            {
                Assert(p.IdPaquete != null, "Paquete.IdPaquete != null");
                Assert(p.Descripcion != null, "Paquete.Descripcion != null");
                Assert(p.Peso == 0.0, "Paquete.Peso == 0.0");
                Assert(p.ValorDeclarado == 0m, "Paquete.ValorDeclarado == 0m");

                string info = p.MostrarInformacion();
                Assert(!string.IsNullOrEmpty(info), "Paquete.MostrarInformacion() returns valid string without throwing");
            }
            else if (instance is Establecimiento e)
            {
                Assert(e.Codigo != null, "Establecimiento.Codigo != null");
                Assert(e.NombreComercial != null, "Establecimiento.NombreComercial != null");
                Assert(e.Despachos != null && e.Despachos.Count == 0, "Establecimiento.Despachos initialized and empty");
                Assert(e.CantidadDespachos == 0, "Establecimiento.CantidadDespachos == 0");
                Assert(e.CalcularTotalFacturable() == 0m, "Establecimiento.CalcularTotalFacturable() == 0m");

                string info = e.MostrarInformacion();
                Assert(!string.IsNullOrEmpty(info), "Establecimiento.MostrarInformacion() returns valid string without throwing");
            }
            else if (instance is Acontecimiento a)
            {
                Assert(a.IdAcontecimiento != null, "Acontecimiento.IdAcontecimiento != null");
                Assert(a.NumeroGuia != null, "Acontecimiento.NumeroGuia != null");
                Assert(a.Tipo != null, "Acontecimiento.Tipo != null");

                string info = a.MostrarInformacion();
                Assert(!string.IsNullOrEmpty(info), "Acontecimiento.MostrarInformacion() returns valid string without throwing");
            }
            else if (instance is RegistroInconsistencia r)
            {
                Assert(r.ArchivoOrigen != null, "RegistroInconsistencia.ArchivoOrigen != null");
                Assert(r.IdentificadorReferencia != null, "RegistroInconsistencia.IdentificadorReferencia != null");
                Assert(r.Tipo != null, "RegistroInconsistencia.Tipo != null");
                Assert(r.Motivo != null, "RegistroInconsistencia.Motivo != null");

                string info = r.MostrarInformacion();
                Assert(!string.IsNullOrEmpty(info), "RegistroInconsistencia.MostrarInformacion() returns valid string without throwing");
            }
        }
    }

    private static void TestChallenge2_ZeroReadonlyKeyword()
    {
        Console.WriteLine("\n--- CHALLENGE 2: Zero 'readonly' in Entire NexoLog Project ---");

        string currentDir = AppContext.BaseDirectory;
        string nexoLogSrcDir = Path.GetFullPath(Path.Combine(currentDir, "..", "..", "..", "..", "NexoLog"));

        if (!Directory.Exists(nexoLogSrcDir))
        {
            Assert(false, "Locate NexoLog source directory", $"Not found at {nexoLogSrcDir}");
            return;
        }

        string[] csFiles = Directory.GetFiles(nexoLogSrcDir, "*.cs", SearchOption.AllDirectories);
        int readonlyFoundCount = 0;
        Regex readonlyRegex = new Regex(@"\breadonly\b", RegexOptions.Compiled);

        foreach (string file in csFiles)
        {
            // Skip obj or bin directories
            if (file.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar) ||
                file.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar))
            {
                continue;
            }

            string[] lines = File.ReadAllLines(file);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                // Check if readonly is in code, not in a comment explaining prohibition
                string trimmed = line.Trim();
                if (trimmed.StartsWith("//") || trimmed.StartsWith("/*") || trimmed.StartsWith("*"))
                {
                    continue;
                }

                if (readonlyRegex.IsMatch(line))
                {
                    readonlyFoundCount++;
                    Console.WriteLine($"   [READONLY DETECTED] {Path.GetFileName(file)}:{i + 1} -> {trimmed}");
                }
            }
        }

        Assert(readonlyFoundCount == 0, "No 'readonly' keyword in any NexoLog source file", $"Found {readonlyFoundCount} instances");
    }

    private static void TestChallenge3_EmpresaStructuralIsolation()
    {
        Console.WriteLine("\n--- CHALLENGE 3: Empresa.cs Structural and Architectural Isolation ---");

        string currentDir = AppContext.BaseDirectory;
        string empresaFile = Path.GetFullPath(Path.Combine(currentDir, "..", "..", "..", "..", "NexoLog", "Empresa.cs"));

        Assert(File.Exists(empresaFile), "Empresa.cs file exists", empresaFile);
        if (!File.Exists(empresaFile)) return;

        string[] lines = File.ReadAllLines(empresaFile);
        int lineCount = lines.Length;
        Console.WriteLine($"   Empresa.cs line count: {lineCount}");

        Assert(lineCount >= 80 && lineCount <= 200, "Empresa.cs line count is between 80 and 200 lines", $"Actual: {lineCount}");

        // Check using directives
        int systemIoUsings = 0;
        int dtosUsings = 0;
        bool hasEmpresaClass = false;

        foreach (string line in lines)
        {
            string trimmed = line.Trim();
            if (trimmed.StartsWith("using System.IO"))
            {
                systemIoUsings++;
            }
            if (trimmed.StartsWith("using NexoLog.DTOs") || trimmed.StartsWith("using DTOs"))
            {
                dtosUsings++;
            }
            if (trimmed.Contains("public class Empresa"))
            {
                hasEmpresaClass = true;
            }
        }

        Assert(systemIoUsings == 0, "Empresa.cs has 0 using directives for System.IO", $"Found: {systemIoUsings}");
        Assert(dtosUsings == 0, "Empresa.cs has 0 using directives for DTOs", $"Found: {dtosUsings}");
        Assert(hasEmpresaClass, "Empresa.cs maintains exact class name 'Empresa'");

        // Verify methods are purely domain orchestrators
        Type empresaType = typeof(Empresa);
        Assert(empresaType.GetMethod("RegistrarEstablecimiento") != null, "Empresa.RegistrarEstablecimiento exists");
        Assert(empresaType.GetMethod("RegistrarDespacho") != null, "Empresa.RegistrarDespacho exists");
        Assert(empresaType.GetMethod("RegistrarSeguimientos") != null, "Empresa.RegistrarSeguimientos exists");
        Assert(empresaType.GetMethod("GenerarInformeOperativo") != null, "Empresa.GenerarInformeOperativo exists");
        Assert(empresaType.GetMethod("GenerarInformeFinanciero") != null, "Empresa.GenerarInformeFinanciero exists");
        Assert(empresaType.GetMethod("GenerarInformeInconsistencias") != null, "Empresa.GenerarInformeInconsistencias exists");
    }

    private static void TestChallenge4_MapToDomainRobustness()
    {
        Console.WriteLine("\n--- CHALLENGE 4: Robustness of MapToDomain on Edge Cases ---");

        // 1. PaqueteJsonDto.MapToDomain
        {
            PaqueteJsonDto dtoNullId = new PaqueteJsonDto { IdPaquete = null, Peso = 2.5, ValorDeclarado = 1000m };
            bool threwExpected = false;
            try
            {
                dtoNullId.MapToDomain();
            }
            catch (NexoLogException)
            {
                threwExpected = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Threw unexpected exception: {ex.GetType().Name}: {ex.Message}");
            }
            Assert(threwExpected, "PaqueteJsonDto.MapToDomain throws NexoLogException on null IdPaquete");

            PaqueteJsonDto dtoEmptyId = new PaqueteJsonDto { IdPaquete = "   ", Peso = 2.5, ValorDeclarado = 1000m };
            threwExpected = false;
            try
            {
                dtoEmptyId.MapToDomain();
            }
            catch (NexoLogException)
            {
                threwExpected = true;
            }
            Assert(threwExpected, "PaqueteJsonDto.MapToDomain throws NexoLogException on whitespace IdPaquete");

            PaqueteJsonDto dtoZeroPeso = new PaqueteJsonDto { IdPaquete = "P-01", Peso = 0.0, ValorDeclarado = 1000m };
            threwExpected = false;
            try
            {
                dtoZeroPeso.MapToDomain();
            }
            catch (NexoLogException)
            {
                threwExpected = true;
            }
            Assert(threwExpected, "PaqueteJsonDto.MapToDomain throws NexoLogException on zero weight");

            PaqueteJsonDto dtoNegativePeso = new PaqueteJsonDto { IdPaquete = "P-01", Peso = -3.2, ValorDeclarado = 1000m };
            threwExpected = false;
            try
            {
                dtoNegativePeso.MapToDomain();
            }
            catch (NexoLogException)
            {
                threwExpected = true;
            }
            Assert(threwExpected, "PaqueteJsonDto.MapToDomain throws NexoLogException on negative weight");

            PaqueteJsonDto dtoValid = new PaqueteJsonDto { IdPaquete = "P-100", Descripcion = "Caja", Peso = 4.5, ValorDeclarado = 50000m };
            Paquete p = dtoValid.MapToDomain();
            Assert(p != null && p.IdPaquete == "P-100" && p.Peso == 4.5, "PaqueteJsonDto.MapToDomain succeeds on valid data");
        }

        // 2. PedidoJsonDto.MapToDomain
        {
            PedidoJsonDto dtoNullGuia = new PedidoJsonDto { NumeroGuia = null, Modalidad = "ESTANDAR", DistanciaKm = 10.0 };
            bool threwExpected = false;
            try
            {
                dtoNullGuia.MapToDomain("COM-01");
            }
            catch (NexoLogException)
            {
                threwExpected = true;
            }
            Assert(threwExpected, "PedidoJsonDto.MapToDomain throws NexoLogException on null NumeroGuia");

            PedidoJsonDto dtoEmptyGuia = new PedidoJsonDto { NumeroGuia = "   ", Modalidad = "ESTANDAR", DistanciaKm = 10.0 };
            threwExpected = false;
            try
            {
                dtoEmptyGuia.MapToDomain("COM-01");
            }
            catch (NexoLogException)
            {
                threwExpected = true;
            }
            Assert(threwExpected, "PedidoJsonDto.MapToDomain throws NexoLogException on whitespace NumeroGuia");

            PedidoJsonDto dtoNoPaquetes = new PedidoJsonDto { NumeroGuia = "NX-9999", Modalidad = "ESTANDAR", DistanciaKm = 10.0, Paquetes = new List<PaqueteJsonDto>() };
            threwExpected = false;
            try
            {
                dtoNoPaquetes.MapToDomain("COM-01");
            }
            catch (NexoLogException)
            {
                threwExpected = true;
            }
            Assert(threwExpected, "PedidoJsonDto.MapToDomain throws NexoLogException on empty paquetes list");

            PedidoJsonDto dtoNullPaquetes = new PedidoJsonDto { NumeroGuia = "NX-9999", Modalidad = "ESTANDAR", DistanciaKm = 10.0, Paquetes = null };
            threwExpected = false;
            try
            {
                dtoNullPaquetes.MapToDomain("COM-01");
            }
            catch (NexoLogException)
            {
                threwExpected = true;
            }
            Assert(threwExpected, "PedidoJsonDto.MapToDomain throws NexoLogException on null paquetes list");

            PedidoJsonDto dtoUnknownModalidad = new PedidoJsonDto
            {
                NumeroGuia = "NX-9999",
                Modalidad = "TELETRANSPORTACION",
                DistanciaKm = 10.0,
                Paquetes = new List<PaqueteJsonDto> { new PaqueteJsonDto { IdPaquete = "P-1", Peso = 1.0 } }
            };
            threwExpected = false;
            try
            {
                dtoUnknownModalidad.MapToDomain("COM-01");
            }
            catch (NexoLogException)
            {
                threwExpected = true;
            }
            Assert(threwExpected, "PedidoJsonDto.MapToDomain throws NexoLogException on unknown modalidad");

            PedidoJsonDto dtoZeroDistancia = new PedidoJsonDto
            {
                NumeroGuia = "NX-9999",
                Modalidad = "ESTANDAR",
                DistanciaKm = 0.0,
                Paquetes = new List<PaqueteJsonDto> { new PaqueteJsonDto { IdPaquete = "P-1", Peso = 1.0 } }
            };
            threwExpected = false;
            try
            {
                dtoZeroDistancia.MapToDomain("COM-01");
            }
            catch (NexoLogException)
            {
                threwExpected = true;
            }
            Assert(threwExpected, "PedidoJsonDto.MapToDomain throws NexoLogException on zero distance");

            PedidoJsonDto dtoDuplicatePaquetes = new PedidoJsonDto
            {
                NumeroGuia = "NX-9999",
                Modalidad = "ESTANDAR",
                DistanciaKm = 15.0,
                Paquetes = new List<PaqueteJsonDto>
                {
                    new PaqueteJsonDto { IdPaquete = "P-SAME", Peso = 1.0 },
                    new PaqueteJsonDto { IdPaquete = "p-same", Peso = 2.0 }
                }
            };
            threwExpected = false;
            try
            {
                dtoDuplicatePaquetes.MapToDomain("COM-01");
            }
            catch (NexoLogException)
            {
                threwExpected = true;
            }
            Assert(threwExpected, "PedidoJsonDto.MapToDomain throws NexoLogException on duplicate package ID");

            PedidoJsonDto dtoProgramadoInvalidDate = new PedidoJsonDto
            {
                NumeroGuia = "NX-9999",
                Modalidad = "PROGRAMADO",
                FechaProgramada = "FECHA-INVALIDA",
                HoraInicio = "08:00",
                HoraFin = "12:00",
                DistanciaKm = 10.0,
                Paquetes = new List<PaqueteJsonDto> { new PaqueteJsonDto { IdPaquete = "P-1", Peso = 1.0 } }
            };
            threwExpected = false;
            try
            {
                dtoProgramadoInvalidDate.MapToDomain("COM-01");
            }
            catch (NexoLogException)
            {
                threwExpected = true;
            }
            Assert(threwExpected, "PedidoJsonDto.MapToDomain throws NexoLogException on invalid programado date");

            PedidoJsonDto dtoProgramadoInvalidHours = new PedidoJsonDto
            {
                NumeroGuia = "NX-9999",
                Modalidad = "PROGRAMADO",
                FechaProgramada = "2026-11-10",
                HoraInicio = "18:00",
                HoraFin = "10:00", // Start after end!
                DistanciaKm = 10.0,
                Paquetes = new List<PaqueteJsonDto> { new PaqueteJsonDto { IdPaquete = "P-1", Peso = 1.0 } }
            };
            threwExpected = false;
            try
            {
                dtoProgramadoInvalidHours.MapToDomain("COM-01");
            }
            catch (NexoLogException)
            {
                threwExpected = true;
            }
            Assert(threwExpected, "PedidoJsonDto.MapToDomain throws NexoLogException on invalid hour window (start >= end)");
        }

        // 3. AcuerdoDto.CrearTarifa & MapToDomain
        {
            AcuerdoDto dtoNullTarifa = new AcuerdoDto { CodigoComercio = "COM-99", TipoTarifa = null, TarifaBase = 5000m };
            bool threwExpected = false;
            try
            {
                dtoNullTarifa.CrearTarifa();
            }
            catch (NexoLogException)
            {
                threwExpected = true;
            }
            Assert(threwExpected, "AcuerdoDto.CrearTarifa throws NexoLogException on null TipoTarifa");

            AcuerdoDto dtoUnknownTarifa = new AcuerdoDto { CodigoComercio = "COM-99", TipoTarifa = "GRATIS", TarifaBase = 5000m };
            threwExpected = false;
            try
            {
                dtoUnknownTarifa.CrearTarifa();
            }
            catch (NexoLogException)
            {
                threwExpected = true;
            }
            Assert(threwExpected, "AcuerdoDto.CrearTarifa throws NexoLogException on unknown TipoTarifa");

            AcuerdoDto dtoPeso = new AcuerdoDto { CodigoComercio = "COM-01", TipoTarifa = "PESO", TarifaBase = 3500m };
            ITarifa tPeso = dtoPeso.CrearTarifa();
            Assert(tPeso is TarifaPorPeso, "AcuerdoDto.CrearTarifa returns TarifaPorPeso");

            AcuerdoDto dtoDist = new AcuerdoDto { CodigoComercio = "COM-02", TipoTarifa = "DISTANCIA", TarifaBase = 2500m };
            ITarifa tDist = dtoDist.CrearTarifa();
            Assert(tDist is TarifaPorDistancia, "AcuerdoDto.CrearTarifa returns TarifaPorDistancia");

            AcuerdoDto dtoFija = new AcuerdoDto { CodigoComercio = "COM-03", TipoTarifa = "FIJA", TarifaBase = 18500m };
            ITarifa tFija = dtoFija.CrearTarifa();
            Assert(tFija is TarifaFija, "AcuerdoDto.CrearTarifa returns TarifaFija");

            AcuerdoDto dtoFull = new AcuerdoDto { CodigoComercio = "COM-01", NombreComercio = "Caribe", TipoTarifa = "PESO", TarifaBase = 3500m };
            Establecimiento est = dtoFull.MapToDomain();
            Assert(est != null && est.Codigo == "COM-01" && est.TarifaAcordada != null, "AcuerdoDto.MapToDomain creates valid Establecimiento");
        }

        // 4. AcontecimientoJsonDto.MapToDomain
        {
            AcontecimientoJsonDto dtoNullId = new AcontecimientoJsonDto { IdAcontecimiento = null, NumeroGuia = "NX-1001", Tipo = "REC" };
            bool threwExpected = false;
            try
            {
                dtoNullId.MapToDomain();
            }
            catch (NexoLogException)
            {
                threwExpected = true;
            }
            Assert(threwExpected, "AcontecimientoJsonDto.MapToDomain throws NexoLogException on null IdAcontecimiento");

            AcontecimientoJsonDto dtoNullGuia = new AcontecimientoJsonDto { IdAcontecimiento = "EVT-1", NumeroGuia = null, Tipo = "REC" };
            threwExpected = false;
            try
            {
                dtoNullGuia.MapToDomain();
            }
            catch (NexoLogException)
            {
                threwExpected = true;
            }
            Assert(threwExpected, "AcontecimientoJsonDto.MapToDomain throws NexoLogException on null NumeroGuia");

            AcontecimientoJsonDto dtoInvalidTipo = new AcontecimientoJsonDto { IdAcontecimiento = "EVT-1", NumeroGuia = "NX-1001", Tipo = "VOLANDO" };
            threwExpected = false;
            try
            {
                dtoInvalidTipo.MapToDomain();
            }
            catch (NexoLogException)
            {
                threwExpected = true;
            }
            Assert(threwExpected, "AcontecimientoJsonDto.MapToDomain throws NexoLogException on invalid event type");

            AcontecimientoJsonDto dtoValidRec = new AcontecimientoJsonDto
            {
                IdAcontecimiento = "EVT-100",
                NumeroGuia = "NX-1001",
                Tipo = "rec",
                FechaHora = new DateTime(2026, 11, 8, 10, 0, 0)
            };
            Acontecimiento ac = dtoValidRec.MapToDomain();
            Assert(ac != null && ac.Tipo == "REC" && ac.IdAcontecimiento == "EVT-100", "AcontecimientoJsonDto.MapToDomain succeeds on valid data (normalizes lowercase type)");
        }
    }

    private static void TestExtra_DespachoEsIdenticoA()
    {
        Console.WriteLine("\n--- EXTRA TEST: Despacho.EsIdenticoA Business Deduplication ---");

        Despacho d1 = new Despacho("NX-1001", "COM-01", "Juan", "Barranquilla", 15.0, "TransNorte");
        d1.AgregarPaquete(new Paquete("P-1", "Caja", 2.5, 10000m));

        Despacho d2Identical = new Despacho("NX-1001", "COM-01", "Juan", "Barranquilla", 15.0, "TransNorte");
        d2Identical.AgregarPaquete(new Paquete("P-1", "Caja", 2.5, 10000m));

        Despacho d3DifferentDest = new Despacho("NX-1001", "COM-01", "Pedro", "Barranquilla", 15.0, "TransNorte");
        d3DifferentDest.AgregarPaquete(new Paquete("P-1", "Caja", 2.5, 10000m));

        Despacho d4DifferentWeight = new Despacho("NX-1001", "COM-01", "Juan", "Barranquilla", 15.0, "TransNorte");
        d4DifferentWeight.AgregarPaquete(new Paquete("P-1", "Caja", 4.0, 10000m));

        Assert(d1.EsIdenticoA(d2Identical), "d1.EsIdenticoA(d2Identical) == true");
        Assert(!d1.EsIdenticoA(d3DifferentDest), "d1.EsIdenticoA(d3DifferentDest) == false (different recipient)");
        Assert(!d1.EsIdenticoA(d4DifferentWeight), "d1.EsIdenticoA(d4DifferentWeight) == false (different weight)");
        Assert(!d1.EsIdenticoA(null), "d1.EsIdenticoA(null) == false");
    }

    private static void TestExtra_InformeCrearEncapsulation()
    {
        Console.WriteLine("\n--- EXTRA TEST: Informe Factory Encapsulation ---");

        Establecimiento est = new Establecimiento("COM-01", "Comercio Caribe", new TarifaPorPeso(3500m));
        Despacho d = new Despacho("NX-1001", "COM-01", "Juan", "Barranquilla", 15.0, "TransNorte");
        d.AsignarModalidad(new ServicioEstandar());
        d.AgregarPaquete(new Paquete("P-1", "Caja", 3.7, 10000m));
        
        // Process lifecycle to Entregado
        d.ProcesarAcontecimiento(new Acontecimiento("E-1", "NX-1001", "REC", new DateTime(2026, 11, 8, 8, 0, 0)));
        d.ProcesarAcontecimiento(new Acontecimiento("E-2", "NX-1001", "RUT", new DateTime(2026, 11, 8, 9, 0, 0)));
        d.ProcesarAcontecimiento(new Acontecimiento("E-3", "NX-1001", "ENT", new DateTime(2026, 11, 8, 11, 0, 0)));
        
        est.AgregarDespacho(d);

        InformeOperativo infOp = InformeOperativo.Crear(new[] { d }, new[] { est });
        Assert(infOp != null, "InformeOperativo.Crear returns instance");
        Assert(infOp.TotalDespachos == 1, "InformeOperativo.TotalDespachos == 1");
        Assert(infOp.TotalEntregados == 1, "InformeOperativo.TotalEntregados == 1");
        Assert(infOp.PorcentajeCumplimientoSobreEntregados == 100.0, "InformeOperativo.PorcentajeCumplimientoSobreEntregados == 100.0%");

        InformeFinanciero infFin = InformeFinanciero.Crear(new[] { est }, new[] { d });
        Assert(infFin != null, "InformeFinanciero.Crear returns instance");
        Assert(infFin.GranTotalFacturado == 12950m, "InformeFinanciero.GranTotalFacturado == $12,950 (3.7kg * 3500)");

        RegistroInconsistencia inc = new RegistroInconsistencia("test.json", "NX-999", "DatoInvalido", "Test error", DateTime.Now);
        InformeInconsistencias infInc = InformeInconsistencias.Crear(new[] { inc });
        Assert(infInc != null, "InformeInconsistencias.Crear returns instance");
        Assert(infInc.TotalInconsistencias == 1, "InformeInconsistencias.TotalInconsistencias == 1");
    }
}
