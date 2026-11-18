# Memoria Operativa y Reglas de Cátedra — POO C# (`POO-3C-IA`)

Este documento es la **única fuente de verdad** normativa, arquitectónica y estilística del repositorio `POO-3C-IA` (**Programación Orientada a Objetos en C# / .NET 10** — Tercer Cuatrimestre de Ingeniería en Ciencia de Datos, Universidad del Magdalena; docente Ing. Andrés Eduardo Paternina Ariza Esp.). Antigravity lo carga automáticamente en cada sesión nueva; toda consulta, planificación, código o diagrama debe adherirse estrictamente a estas directrices.

---

## 1. Filosofía de Trabajo, Rol Crítico y Flujo en Antigravity

1. **Rol de la IA (Tutor y Tech Lead Crítico):**
   - Actúa como par técnico riguroso, nunca como autómata complaciente de "sí a todo". Cuestiona constructivamente decisiones que violen la POO, la simplicidad o las reglas de cátedra, advirtiendo riesgos antes de implementar.
   - Explica conceptualmente el "por qué" de los cambios relevantes en español natural, formal y claro.
2. **Simplicidad Académica y Código 100% Defendible (KISS y YAGNI):**
   - La sustentación es una charla casual breve (< 10 minutos, 3 preguntas individuales). Cada clase, propiedad y método debe poder justificarse con soltura por un estudiante de pregrado sin recurrir a jerga ni capas innecesarias.
   - **Cero sobreingeniería:** prohibido introducir patrones empresariales o capas gratuitas (CQRS, MediatR, contenedores DI, factorías vacías o clases intermediarias sin lógica) salvo exigencia explícita del enunciado.
3. **Flujo Nativo en Antigravity:**
   - Aprovechar los comandos nativos (`/plan`, `/grill-me`, `/teamwork`, `/boost`, `/learn`) para diseñar, entrevistar requisitos, ejecutar o consolidar reglas antes de tocar archivos.
   - Todo cambio de código se verifica compilando limpio (`dotnet build`: 0 errores, 0 advertencias) y validando ejecución funcional en consola (`dotnet run`).

---

## 2. Reglas de Oro en C# (Consolidadas con Sesiones 01–19, `ParcialA` y `ParcialB`)

### 2.1. Declaraciones, Tipos y Estructuras de Control
* **Declaraciones explícitas e individuales (cero `var`):** cada variable lleva su tipo explícito en su propia línea independiente. Queda prohibido agrupar (`double a, b;`) y prohibido usar `var`.
* **Cero `enum` (usar constantes):** estados y tipos fijos públicos se modelan con `public const string PascalCase` (ej. `Despacho.EstadoPendiente`); constantes internas de clase/estrategia usan `private const <tipo> _camelCase` (`_precioPorPagina`, `_porcentajeDescuento`).
* **Uso de `switch` comprimido y bifurcaciones:** se permite únicamente el formato comprimido (*switch expression* `=>`) en fábricas, validaciones de transición, mapeadores y menús de consola (prohibido el `switch` tradicional con bloques `case ...: break;`); para otras bifurcaciones de flujo se utiliza `if / else if / else`.
* **Cero `TimeSpan` complejo:** modelar franjas horarias sencillas con tipos primitivos (`int _horaInicio`, `int _horaFin` o texto simple).
* **Comparaciones simples de cadenas:** preferir `==` o `.ToLower() == ...` en lugar de formas verbosas cuando no se requiera comparación cultural especial.

### 2.2. Cero Comentarios en el Código Fuente
* **Prohibición absoluta de comentarios:** cero `/// <summary>`, `<param>`, `//` o `/* */` en clases, interfaces, DTOs, servicios y `Program.cs` (fiel al código oficial del profesor en `Actividad`, `ParcialA` y `ParcialB`).
* El código debe explicarse exclusivamente mediante nombres expresivos de clases, propiedades, variables y métodos.

### 2.3. Constructores Vacíos Limpios y Constructores Públicos
* **Según el tipo de clase:**
  - **Entidades concretas con constructor parametrizado:** incluir **`private <Clase>() { }`** completamente vacío (protege encapsulamiento contra `new Despacho()` incompleto y mantiene compatibilidad técnica/serialización).
  - **Clases base abstractas:** incluir **`protected <Clase>() { }`** completamente vacío.
  - **Estrategias o subtipos sin estado variable:** constructor público vacío (`public TarifaRegular() { }`).
* **Cero inicializaciones duplicadas:** está prohibido inicializar atributos dentro del constructor privado/protegido vacío. Toda inicialización de listas, estado inicial explícito (nunca `null`) y asignación a propiedades validadas viven una sola vez en el constructor público horizontal (1–2 líneas balanceadas de parámetros) o en la declaración de la colección privada.

### 2.4. Encapsulamiento Estricto, Validaciones y Copias Defensivas
* **Cero `readonly`:** prohibida la palabra clave `readonly` en campos, parámetros e interfaces (`IReadOnlyList<T>`, `.AsReadOnly()`).
* **Cero `set` públicos en el dominio:** ninguna entidad de dominio expone `public set`. Las propiedades mutables usan `private set` con validación temprana (guard clauses + `.Trim()` y `.ToUpper()` si aplica) y solo se modifican mediante métodos de negocio (`AsignarTarifa()`, `AsignarModalidad()`, `RegistrarSalidaARuta()`, `CambiarPolitica()`). Las auto-propiedades `{ get; set; }` quedan reservadas exclusivamente para **DTOs**.
* **Copias defensivas en colecciones:** al exponer una lista privada al exterior, devolver copia defensiva mediante propiedad de solo `get` (`get { return new List<Paquete>(_paquetes); }`) y proveer métodos explícitos de inserción con validación de nulo y unicidad (`AgregarPaquete()`, `RegistrarInconsistencia()`, `ExisteCodigo()`). Prohibido mutar listas internas desde fuera (`sys.Inconsistencias.Add(...)` es inválido).
* **Orden interno estandarizado de clase:**
  1. Constantes (`public const` / `private const`)
  2. Campos privados (`_camelCase`, sin inicializaciones redundantes si el constructor los asigna)
  3. Propiedades calculadas breves (`get { return ...; }`)
  4. Constructores (`private`/`protected` vacío + público horizontal)
  5. Propiedades públicas (`get` / `private set` con validación)
  6. Métodos de negocio y `MostrarInformacion()`

```csharp
public class Despacho
{
    public const string EstadoPendiente = "Pendiente";

    private string _numeroGuia;
    private string _estadoActual;
    private List<Paquete> _paquetes;

    public int CantidadPaquetes { get { return _paquetes.Count; } }

    private Despacho() { }

    public Despacho(string numeroGuia, string codigoEstablecimiento, string destinatario, string ciudadDestino, double distanciaKm, string transportadora)
    {
        NumeroGuia = numeroGuia;
        _estadoActual = EstadoPendiente;
        _paquetes = new List<Paquete>();
    }

    public string NumeroGuia
    {
        get { return _numeroGuia; }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NexoLogException("El numero de guia es obligatorio.");
            }
            _numeroGuia = value.Trim();
        }
    }

    public List<Paquete> Paquetes
    {
        get { return new List<Paquete>(_paquetes); }
    }
}
```

### 2.5. Nomenclatura Expresiva y Regla de la "Y" (Responsabilidad Única)
* **Prohibidos nombres vagos:** está prohibido nombrar métodos como `Procesar...`, `Integrar...`, `Manejar...` o `Gestionar...` (incluyendo textos del menú). Todo método debe revelar con exactitud su acción en el dominio (`AgregarAcontecimiento`, `BuscarAcontecimiento`, `TieneMismosDatosQue`, `EsFacturable`).
* **Regla de la "Y":** si un método requiere la conjunción *"Y"* en su nombre (`ExtraerYAgregar...`) porque hace dos tareas distintas, debe dividirse en dos métodos cohesivos separados donde uno orqueste al otro.
* **Método de salida en consola:** usar `MostrarInformacion()` en las entidades que proyecten detalle en pantalla; evitar sobrecargar `ToString()` para formateos complejos.

### 2.6. Polimorfismo Real, DTOs (Sesión 17), Patrones (Sesión 19) y Resiliencia
* **Polimorfismo real en interfaces (cero chequeos `is Subtipo`):** si una capa necesita un dato o cálculo común de varias implementaciones, ese miembro debe vivir en la interfaz (`ITarifa.TipoTarifa`, `ITarifa.ValorBase`), prohibiéndose cadenas `if (x is TarifaPorPeso) ... else if ...`.
* **Sesión 17 — DTOs y Mappers ("Los objetos de dominio nunca se serializan"):**
  - Ingesta: archivo (JSON/XML) $\rightarrow$ DTO unificado (`AcuerdoDto`, `PedidoDto`, `PaqueteDto`, `AcontecimientoDto`) $\rightarrow$ `MapToDomain(...)` $\rightarrow$ entidad validada.
  - Salida/Reportes: `Empresa.MapToDto()` $\rightarrow$ `InformeConsolidadoDto` (`{ get; set; }`, listas serializables nativas para `JsonSerializer` y `XmlSerializer`).
  - **Prohibidos los tipos anónimos (`new { ... }`)**.
* **Sesión 19 — `Strategy + Factory Method`:**
  - Estrategias detrás de interfaces con nombres de dominio puro (`ITarifa`, `IImportadorArchivo<T>`, `IExportadorInforme`) sin sufijos artificiales (`Strategy`).
  - Creación centralizada en fábricas estáticas simétricas (`ImportadorFactory`, `ExportadorFactory`) o método estático `Crear(...)` en clase abstracta base (`ModalidadServicio.Crear(...)`), evitando lógica de instanciación dispersa en `Program.cs` o en la ingesta.
  - *Reglas de sintaxis en fábricas y patrones:* usar switch expressions (`=>`) para la selección de estrategias, modalidades y formatos, manteniendo tipos explícitos (cero `var`), cero comentarios y retorno directo de la instancia correspondiente.
* **Dominio puro y rechazo a clases anémicas:** `Empresa` coordina colecciones del negocio, consolida sus reportes y expone `MapToDto()` (estilo `Solicitud` en CopiaExpress) sin lectura de disco ni carpetas intermedias innecesarias. Registros históricos sin comportamiento divergente (`Acontecimiento`) no se dividen en subclases vacías; `Despacho` gobierna su máquina de estados en `AgregarAcontecimiento`.
* **Excepciones y UTF-8:** usar excepciones de dominio heredando de `Exception` cuando la regla o el enunciado las exija (`NexoLogException`, o `Excepciones.cs` en parciales) y `ArgumentException`/`Exception` en ejercicios sencillos; especificar siempre `Encoding.UTF8` explícito en consola y archivos.

---

## 3. Reglas de Modelado UML de Clases (Draw.io)

* **Cero rombos:** el docente no usa rombos de agregación ni composición (Sesión 15 diap. 4, 6 y 7). Toda relación estructural se dibuja con líneas de asociación simples y multiplicidades explícitas (`1`, `0..*`, `1..*`), herencia con triángulo cerrado vacío y realización con línea discontinua y triángulo vacío.
* **Cero prefijo `_`, cero `= valor` y cero `{override}`:** visibilidad privada solo con `-` en camelCase (`- codigo: string`); sin valores fijos asignados (`= 2000`) ni etiquetas `{override}` redundantes.
* **Orientación vertical, simetría y fidelidad 1:1:** interfaces y clases padre siempre **arriba** y sus implementaciones/hijas **abajo**; fábricas (`ImportadorFactory`, `ExportadorFactory`) a un costado con dependencia discontinua (`«use»`) y simetría entre entrada y salida. **Regla de Sesión 15 diap. 14:** no agregar cajas, factorías vacías ni clases puente (`AcuerdoComercial`) que no existan en C#.

---

## 4. Política Documental, PDFs y Cero Residuos

* **Máximo dos documentos explicativos en la raíz:** [`README.md`](./README.md) y [`Guia_Arquitectura_NexoLog.pdf`](./Guia_Arquitectura_NexoLog.pdf). Archivos de gobernanza y licencia (`AGENTS.md`, `LICENSE`) quedan exentos de ese límite.
* **Carpeta `04_Proyecto_Final/` limpia:** contiene únicamente `NexoLog/` (código C# y `DatosPrueba/` JSON/XML) y `Diagrama_UML_NexoLog.drawio(.pdf)`. Queda prohibido crear archivos `.md`, `.gitkeep` obsoletos o proyectos residuales de testing (`NexoLog.Tests/`) dentro de subcarpetas de proyecto.
* **Guías PDF (`Guia_Arquitectura_NexoLog.pdf`):** tono directo, reflexivo y cómodo para compañeros de **Ingeniería en Ciencia de Datos (3.er cuatrimestre)**, sin preámbulos vacíos ni tono doctoral; letra de `10.5–11 pt` con interlineado amplio (`14.5–15 pt`), tarjetas Modern UI nativas y callouts coloreados en ReportLab; **prohibido incrustar diagramas hechos con Matplotlib**.

---

## 5. Mapa del Repositorio y Especificaciones de NexoLog S.A.S.

### 5.1. Organización del Repositorio
* **Documentos rectores (raíz):** `00_MicroDiseño - ICD.pdf`, `00_Cronograma Detallado de Clases.pdf`, `00_Proyecto Final POO 3C.pdf`.
* **Diapositivas (`Sesion_XX - ...pdf` en raíz):** `Sesion_01` (Introducción POO), `Sesion_02` (Colecciones), `Sesion_03` (Clases/Abstracción), `Sesion_05` (Encapsulamiento), `Sesion_07` (Construcción de clase), `Sesion_09` (Herencia), `Sesion_10` (Relaciones), `Sesion_12` (Polimorfismo), `Sesion_14` (Abstractas/Interfaces/Excepciones), `Sesion_15` (UML), `Sesion_17` (Serialización + DTO + Deserialización), `Sesion_19` (Strategy + Factory Method), `Sesion_21` (Pruebas Unitarias).
* **Carpetas funcionales:**
  - `01_Ejemplos_Clase/`: `Sesion_03_Gimnasio`, `Sesion_05_Encapsulamiento_Cine`, `Sesion_07_Vehiculo`, `Sesion_10_Herencia_Empleados`, `Sesion_12_Polimorfismo_Hotel`, `Sesion_19_Parciales_Finales_Y_Ejemplo_Clase` (`Actividad/`, `ParcialA/`, `ParcialB/`).
  - `02_Actividades_y_Talleres/`: `Sesion_04_Taller_Cine_CRC`, `Sesion_06_Actividad_MoviCar`, `Sesion_13_Actividad_ClasesAbstractas`, `Sesion_15_Taller_Veterinaria_UML`, `Sesion_16_Taller_Alquiler_Vehiculos`, `Practica Segundo Parcial`.
  - `03_Evaluaciones/`: `Sesion_08_Parcial_01`, `Sesion_11_Recuperacion_Parcial_01_EcoRide`.
  - `04_Proyecto_Final/`: `NexoLog/` y `Diagrama_UML_NexoLog.drawio(.pdf)`.
  - `InformesExportados/`: `informes_nexolog.json`, `informes_nexolog.xml`.

### 5.2. Reglas de Negocio y Cifras de Control (Golden Baseline de NexoLog — Datos Oficiales)
* **Acuerdos comerciales (`DatosPrueba/acuerdos.json`):** `COM-01` Tarifa por peso (`$3.500/kg`), `COM-02` Tarifa fija (`$18.000`), `COM-03` Tarifa por distancia (`$1.200/km`).
* **Caso de control `NX-1001`:** `2.5 kg + 1.2 kg = 3.7 kg` $\times$ `$3.500/kg` $=$ **`$12.950`** exactos.
* **Ordenamiento temporal obligatorio:** antes de ejecutar `Despacho.AgregarAcontecimiento()`, ordenar ascendentemente los acontecimientos por `FechaHora` para reconstruir eventos desordenados en archivo.
* **SLA y facturación:**
  - Porcentaje SLA = `(Entregados a Tiempo / Total Entregados) × 100` (`9 / 17 = 52.94%`). Pendientes (`1`), en distribución (`2`) y devueltos (`4`) se excluyen del denominador.
  - Solo los despachos con estado final `Entregado` (`17`) habilitan cobro (`Gran Total Facturado: $285.800` — `COM-01: $77.000`, `COM-02: $90.000`, `COM-03: $118.800`); un entregado fuera de tiempo sigue siendo facturable, aunque incumple SLA.
* **Resiliencia y exportación polimórfica:** datos corruptos, duplicados y secuencias incompatibles generan `RegistroInconsistencia` sin abortar la ejecución (`24` despachos válidos, `29` paquetes, `71.50 kg`, `18` inconsistencias auditadas) y se exportan vía `InconsistenciaDto` + `InformeConsolidadoDto` con `ImportadorFactory` / `ExportadorFactory` bajo `InformesExportados/` en `Encoding.UTF8`.
