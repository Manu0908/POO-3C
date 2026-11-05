# Preguntas Orientadoras para Asesoría con el Docente
**Proyecto Final: NexoLog S.A.S. — Sistema de Integración Logística**  
**Asignatura:** Programación Orientada a Objetos (C# / .NET)

Este documento reúne un conjunto de inquietudes técnicas, reflexivas y bien fundamentadas para discutir con el profesor en asesoría o clase. Las preguntas están planteadas desde la perspectiva de un estudiante que ya comprende el modelado de clases básicas (atributos, validaciones, herencia, polimorfismo e interfaces), pero busca validar el diseño de las capas más complejas (orquestación, flujo de consola, resiliencia y deserialización).

---

## 1. Sobre la Orquestación y la Clase Central (`Empresa`)

* **Contexto de diseño:**  
  En nuestro proyecto, las entidades de dominio puro (`Despacho`, `Paquete`, `Establecimiento`, `Acontecimiento`) se protegen a sí mismas mediante validaciones en sus constructores y setters. Sin embargo, surge la necesidad de tener un componente que reciba los 5 archivos heterogéneos (JSON y XML), coordine la carga de los comercios remitentes, asocie los acuerdos comerciales y resuelva el orden temporal de los acontecimientos. Hemos modelado esta responsabilidad en una clase llamada **`Empresa`** (que representa la oficina central de NexoLog).

* **Preguntas para el docente:**
  1. *“Profesor, para evitar que la clase central (`Empresa`) se convierta en una ‘clase todopoderosa’ que haga demasiadas cosas, delegamos la lectura de archivos a lectores independientes y la máquina de estados a cada `Despacho`. ¿Considera adecuado que la vinculación de referencias (por ejemplo, buscar a qué comercio pertenece cada guía y ordenar los eventos) viva en la entidad `Empresa`, o sugeriría separar esto en un servicio intermedio?”*
  2. *“En proyectos empresariales se habla a veces de servicios o coordinadores, pero en el ámbito académico de este curso, ¿es preferible que esta clase se llame `Empresa`, `SistemaNexoLog` o `CoordinadorLogistico` para que su propósito sea evidente y defendible?”*

---

## 2. Sobre la Responsabilidad de `Program.cs` y la Consola

* **Contexto de diseño:**  
  El enunciado pide una aplicación de consola con 6 opciones funcionales (cargar archivos, procesar e integrar, ver informe operativo, financiero, inconsistencias y exportar). En nuestro `Program.cs` tenemos un bucle `while` con un menú numérico que captura la opción del usuario e invoca a los métodos de la clase orquestadora.

* **Preguntas para el docente:**
  1. *“En la evaluación arquitectónica de la cátedra, ¿cuánta lógica considera aceptable que resida en `Program.cs`? ¿Es correcto que el `Main` contenga métodos locales para formatear las tablas de texto en pantalla (`Console.WriteLine`), o la rúbrica valora que incluso la visualización esté separada en una clase tipo `ConsolaVista`?”*
  2. *“Para el menú de opciones, ¿prefiere el uso de cadenas condicionales tradicionales `if / else if / else` o estructuras de selección múltiple, considerando la claridad y defendibilidad del código?”*

---

## 3. Sobre la Resiliencia y el Informe de Inconsistencias

* **Contexto de diseño:**  
  Los archivos de prueba contienen intencionalmente registros corruptos: paquetes con peso menor o igual a cero, guías repetidas con información contradictoria o acontecimientos que referencian despachos que no existen. El enunciado estipula: *“La existencia de información incorrecta no deberá impedir que se generen los informes correspondientes a los registros válidos”*.

* **Preguntas para el docente:**
  1. *“Profe, para cumplir con el requisito de que un dato inválido no tumbe el procesamiento de los demás registros, en los constructores de dominio lanzamos nuestra excepción de negocio (`NexoLogException`), y en la fase de lectura capturamos esa excepción en un bloque `try/catch` para registrarla en la lista de inconsistencias sin abortar el bucle. ¿Este mecanismo de resiliencia refleja la práctica esperada por la cátedra, o preferiría que las validaciones de frontera no utilicen excepciones sino retornos booleanos?”*
  2. *“Cuando una guía se repite exactamente idéntica, el enunciado pide no duplicar la operación; pero cuando la misma guía trae datos distintos, pide reportar un conflicto. ¿Es correcto conservar el primer registro procesado y rechazar el segundo registrándolo en la auditoría, o existe alguna política de sobreescritura que debamos contemplar?”*

---

## 4. Sobre la Deserialización Heterogénea (JSON y XML)

* **Contexto de diseño:**  
  La empresa suministra archivos con estructuras diferentes: solicitudes en JSON (`pedidos_caribe.json`) y en XML (`pedidos_andino.xml`), así como seguimientos en ambos formatos.

* **Preguntas para el docente:**
  1. *“Para leer estos formatos heterogéneos sin acoplar las clases de negocio a las etiquetas de los archivos, ¿recomienda utilizar clases sencillas de transferencia de datos (DTOs) que sirvan de puente de deserialización, o espera que las clases de dominio se serialicen/deserialicen de forma directa con anotaciones?”*
  2. *“Respecto a las fechas y horas en los archivos de tracking, notamos que los acontecimientos pueden venir físicamente desordenados en el documento. ¿Es indispensable ordenar la colección por fecha y hora ascendente antes de evaluar las transiciones de estado (`REC` -> `RUT` -> `ENT`), para no rechazar entregas legítimas que quedaron escritas antes de la salida a ruta?”*

---

## 5. Sobre la Justificación Oral de Patrones (Strategy y Factory)

* **Contexto de diseño:**  
  El enunciado solicita la aplicación funcional de los patrones Strategy y Factory. En el código implementamos la interfaz `ITarifa` con tres clases (`TarifaFija`, `TarifaPorPeso`, `TarifaPorDistancia`) para liquidar cobros, y la clase `CreadorLectores` para obtener los lectores de JSON o XML según la extensión.

* **Preguntas para el docente:**
  1. *“Durante la sustentación oral, ¿cuál es el argumento de diseño más contundente que usted espera escuchar sobre Strategy? ¿Enfocarnos en el Principio de Abierto/Cerrado (demostrar que podemos agregar una tarifa por volumen sin modificar `Despacho`) o argumentarlo desde la reducción de estructuras condicionales?”*
  2. *“Para el patrón Factory, ¿le parece adecuado nombrarlo en español de dominio puro como `CreadorLectores` con métodos estáticos de creación, o prefiere que conserve explícitamente el sufijo `Factory` en el nombre de la clase?”*
