# Manifiesto y Reglas de Programación en C# (Guía Portátil para IA)

> **Instrucción para la Inteligencia Artificial:**  
> Este documento contiene el estándar de codificación innegociable del desarrollador. Cuando generes, refactorices o revises código en C#, debes adherirte estrictamente a estas reglas. El objetivo primordial no es la sofisticación sintáctica ni el uso de características avanzadas del lenguaje, sino producir código **limpio, legible, modular, proporcional al problema y 100% defendible en una sustentación universitaria**.

---

## 1. Filosofía de Desarrollo

1. **Cero Sobreingeniería:** Resuelve el problema con la arquitectura más simple y directa posible. No incorpores capas innecesarias, bibliotecas pesadas, contenedores de inyección de dependencias ni patrones empresariales complejos (CQRS, MediatR, Event Sourcing) a menos que se exijan explícitamente.
2. **Código Defendible:** Cada clase, método, atributo y validación debe tener una justificación comprensible por un estudiante de pregrado frente a su profesor evaluador. Si una línea no puede justificarse verbalmente con soltura, esa línea no debe existir.
3. **Nombres de Dominio Puro:** Las clases deben nombrarse por lo que representan en el mundo real o en el negocio. No incorpores sufijos de patrones de diseño en los nombres de las clases (usa `ITarifa` y `TarifaPorPeso` en lugar de `IEstrategiaCobro` o `TarifaPesoStrategy`; usa `CreadorLectores` en lugar de `LectorFactory`).

---

## 2. Declaraciones y Tipos de Datos

1. **Declaraciones Explícitas e Individuales (Regla de Oro):**
   * Queda estrictamente prohibido agrupar declaraciones de variables en una sola línea.
   * Cada variable debe contar con su tipo de dato explícito y residir en su propia línea independiente.
   ```csharp
   // CORRECTO
   double nota1;
   double nota2;
   double nota3;

   // PROHIBIDO
   double nota1, nota2, nota3;
   var a = 1, b = 2;
   ```
2. **Cero `enum` (Usar Constantes `string`):**
   * A menos que el enunciado exija enums explícitamente, modela los estados y tipos fijos como constantes textuales dentro de la clase correspondiente.
   ```csharp
   public class Despacho
   {
       public const string EstadoPendiente = "Pendiente";
       public const string EstadoEnDistribucion = "EnDistribucion";
       public const string EstadoEntregado = "Entregado";
       public const string EstadoDevuelto = "Devuelto";
   }
   ```
3. **Cero `TimeSpan` para franjas horarias sencillas:**
   * Modela intervalos o franjas horarias mediante tipos primitivos simples (enteros de hora militar como `int horaInicio = 14`, `int horaFin = 17` o cadenas de texto), evitando estructuras temporales avanzadas de .NET.

---

## 3. Campos Privados, Propiedades y Encapsulamiento

1. **Campos Privados con Guion Bajo (`_campo`):**
   * Nomenclatura camelCase precedida por un guion bajo (`_codigo`, `_descripcion`).
   * **Cero inicializaciones redundantes:** Si un campo es asignado obligatoriamente en el constructor, no debe inicializarse en la cabecera con valores por defecto vacíos (`string.Empty`, `""` o `0`).
   ```csharp
   // CORRECTO
   private string _codigo;
   private double _peso;

   // INCORRECTO
   private string _codigo = string.Empty;
   private double _peso = 0.0;
   ```
2. **Auto-propiedades vs. Campos Privados (Regla de Mínima Redundancia):**
   * **Sin validación en el setter:** NO declares un campo privado manual. Usa una auto-propiedad compacta:
     ```csharp
     public DateTime FechaDeteccion { get; private set; }
     public string ArchivoOrigen { get; private set; }
     ```
   * **Con validación o regla de negocio:** Declara el campo privado `_campo` y coloca la validación dentro del setter:
     ```csharp
     private double _peso;
     public double Peso
     {
         get { return _peso; }
         private set
         {
             if (value <= 0)
             {
                 throw new NexoLogException("El peso debe ser mayor a 0 kg.");
             }
             _peso = value;
         }
     }
     ```
3. **Propiedades Pequeñas en una Sola Línea:**
   * Getters simples o propiedades calculadas breves deben escribirse en una sola línea:
     ```csharp
     public bool ArchivosCargados { get { return _archivosCargados; } }
     public int CantidadPaquetes { get { return _paquetes.Count; } }
     ```
4. **Prohibición de `readonly` y de `IReadOnlyList<T>`:**
   * Queda estrictamente prohibido el uso de la palabra clave `readonly` en campos, parámetros o interfaces.
   * La protección de solo lectura se modela mediante campos privados convencionales y **propiedades con solo `get`**:
     ```csharp
     private List<Paquete> _paquetes;

     public List<Paquete> Paquetes
     {
         get { return _paquetes; }
     }
     ```

---

## 4. Constructores y Métodos

1. **Constructores Horizontales Compactos:**
   * Los constructores y métodos deben escribirse con sus parámetros en formato horizontal (1 o 2 líneas balanceadas), evitando el formateo vertical en cascada línea por línea:
     ```csharp
     // CORRECTO
     public Paquete(string idPaquete, string descripcion, double peso, decimal valorDeclarado)
     {
         IdPaquete = idPaquete;
         Descripcion = descripcion;
         Peso = peso;
         ValorDeclarado = valorDeclarado;
     }
     ```
2. **Estado Inicial Siempre Explícito (Nunca `null`):**
   * Todo objeto debe nacer en un estado válido predeterminado en su constructor.
   ```csharp
   _estadoActual = EstadoPendiente;
   ```
3. **Métodos Cortos con Responsabilidad Única:**
   * Si un método supera las 20 o 25 líneas, debe dividirse en submétodos privados con nombres directos en español que expresen claramente lo que hacen (`ValidarNoFinalizado()`, `ProcesarRecepcion()`, etc.).
4. **Método de Presentación Uniforme:**
   * Las clases que proyecten información en pantalla deben implementar un método limpio y explícito:
     ```csharp
     public string MostrarInformacion()
     ```
   * Evita sobrecargar `ToString()` para formateos complejos de negocio.

---

## 5. Estructuras de Control y Operadores

1. **Prohibido `switch` (Usar `if / else if / else`):**
   * Todas las decisiones condicionales deben implementarse mediante estructuras tradicionales `if / else if / else`:
     ```csharp
     // CORRECTO
     if (tipo == Acontecimiento.TipoRec)
     {
         ProcesarRecepcion(evt);
     }
     else if (tipo == Acontecimiento.TipoRut)
     {
         ProcesarSalidaRuta(evt);
     }
     ```
2. **Comparaciones Directas de Cadenas (Cero `.Equals(...)` verbosos):**
   * En lugar de usar `string.Equals(a, b, StringComparison.OrdinalIgnoreCase)`, utiliza comparaciones estándar:
     ```csharp
     // CORRECTO
     if (a == b) ...
     if (a.ToLower() == b.ToLower()) ...
     ```

---

## 6. Validación de Datos y Manejo de Excepciones

1. **Validaciones Tempranas (*Guard Clauses*):**
   * Toda cadena de texto debe validarse al inicio con `string.IsNullOrWhiteSpace(valor)`.
   * Todo valor numérico debe respetar límites físicos: cantidades, distancias y pesos estrictamente positivos (`> 0`); valores monetarios no negativos (`>= 0`).
2. **Excepciones de Negocio y Resiliencia en Frontera:**
   * El dominio lanza excepciones personalizadas que heredan de `Exception` (por ejemplo, `NexoLogException`) cuando se violan reglas invariantes.
   * La capa de lectura o integración envuelve las operaciones en bloques `try / catch (NexoLogException ex)` para no detener la ejecución global, convirtiendo el error en un registro de auditoría/inconsistencia y continuando con los registros válidos.
3. **Codificación de Archivos Obligatoria:**
   * Cualquier lectura o escritura sobre archivos de texto, JSON o XML debe especificar explícitamente `System.Text.Encoding.UTF8`.

---

## 7. Orden Interno Estandarizado en Cada Clase

Toda clase en C# debe organizarse en este orden exacto:
1. **Constantes públicas / privadas** (`public const string ...`)
2. **Campos privados** (únicamente aquellos que validan o colecciones internas)
3. **Propiedades calculadas o derivadas breves** (en 1 línea)
4. **Constructores** (en formato horizontal)
5. **Propiedades públicas** (auto-implementadas o con validación en el setter)
6. **Métodos de negocio** y método `MostrarInformacion()`

---

## 8. Comentarios y Documentación en Código

1. **Cero Comentarios XML con Formato HTML:**
   * Queda estrictamente prohibido el uso de etiquetas de documentación XML (`/// <summary>`, `<param>`, etc.).
2. **Código Autoexplicativo:**
   * El código debe entenderse por la claridad de sus nombres. Solo se admiten comentarios estándar de una sola línea (`//`) breves y puntuales para justificar fórmulas matemáticas complejas o restricciones directas del docente.

---

## 9. Reglas de Modelado UML (Draw.io)

1. **Cero Rombos (Convención Estricta de Cátedra):**
   * El docente no utiliza rombos de agregación ni de composición. Toda relación estructural se modela con líneas simples de asociación y sus multiplicidades explícitas (`1`, `*`, `0..*`, `0..1`), herencia con flecha de triángulo vacío hacia el padre y realización de interfaz con línea discontinua y triángulo vacío.
2. **Cero Prefijos `_` en Atributos:**
   * La visibilidad privada se expresa exclusivamente con el signo menos `-` seguido del nombre en camelCase (`- codigo: string`), eliminando el guion bajo redundante de C#.
3. **Cero Asignaciones de Valores y Cero `{override}`:**
   * Prohibido colocar valores fijos en atributos (`= 34`, `= 2000`) o etiquetas `{override}` en métodos polimórficos de clases hijas.
