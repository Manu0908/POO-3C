# POO-3C: Programación y Diseño Orientado a Objetos

Repositorio académico y de desarrollo asistido por Inteligencia Artificial para la asignatura de **Programación Orientada a Objetos (C#)** correspondiente al tercer cuatrimestre universitario.

El propósito central de este repositorio es construir software limpio, modular y progresivo, garantizando que cada línea de código sea 100% comprensible y defendible en sustentaciones académicas orales.

---

## 1. Estructura General del Repositorio

El contenido se organiza de forma cronológica y funcional para facilitar el estudio y la trazabilidad de los temas:

```text
POO-3C/
├── [Documentos Rectores]
│   ├── 00_MicroDiseño - ICD.pdf
│   ├── 00_Cronograma Detallado de Clases.pdf
│   └── 00_Proyecto Final POO 3C.pdf
│
├── [Diapositivas de Clase]                            # Teoría cronológica con padding de dos dígitos
│   ├── Sesion_01 - Introduccion a POO.pdf
│   ├── Sesion_02 - Colecciones en C#.pdf
│   ├── Sesion_03 - Clases, Objetos y Abstraccion.pdf
│   ├── Sesion_05 - Encapsulamiento.pdf
│   ├── Sesion_07 - Generalidades al construir una clase.pdf
│   ├── Sesion_09 - Herencia.pdf
│   ├── Sesion_10 - Relaciones y colaboraciones entre objetos.pdf
│   ├── Sesion_12 - Polimorfismo.pdf
│   ├── Sesion_14 - Clases Abstractas, Interfaces y Excepciones.pdf
│   └── Sesion_15 - UML.pdf
│
├── 01_Ejemplos_Clase/                                 # Ejemplos demostrativos desarrollados en aula
│   ├── Sesion_03_Gimnasio/
│   ├── Sesion_05_Encapsulamiento_Cine/
│   ├── Sesion_07_Vehiculo/
│   ├── Sesion_10_Herencia_Empleados/
│   └── Sesion_12_Polimorfismo_Hotel/
│
├── 02_Actividades_y_Talleres/                         # Talleres evaluados, tarjetas CRC y diagramas
│   ├── Sesion_04_Taller_Cine_CRC/
│   ├── Sesion_06_Actividad_MoviCar/
│   ├── Sesion_13_Actividad_ClasesAbstractas/
│   ├── Sesion_15_Taller_Veterinaria_UML/
│   └── Sesion_16_Taller_Alquiler_Vehiculos/
│
├── 03_Evaluaciones/                                   # Parciales y recuperaciones formales
│   ├── Sesion_08_Parcial_01/
│   └── Sesion_11_Recuperacion_Parcial_01_EcoRide/
│
├── 04_Proyecto_Final/                                 # Proyecto Integrador de Fin de Semestre
│   ├── NexoLog/                                       # Código fuente en C# / .NET 10
│   ├── Diagrama_UML_NexoLog.drawio                   # Diagrama oficial de clases en Draw.io (sin rombos)
│   └── Diagrama_UML_NexoLog.drawio.pdf               # Versión exportada en PDF del diagrama
│
├── Guia_Arquitectura_NexoLog.pdf                      # Guía unificada de arquitectura y decisiones de diseño
├── AGENTS.md                                          # Memoria operativa y marco normativo de agentes
└── REGLAS_PROGRAMACION.md                             # Manifiesto portátil de reglas de estilo en C#
```

---

## 2. Proyecto Final: NexoLog S.A.S.

Sistema de consola en C# / .NET para la integración y análisis de operaciones logísticas multifuente:
* **Entidades del Negocio:** `Empresa`, `Establecimiento`, `Despacho`, `Paquete`, `Acontecimiento`, `RegistroInconsistencia`.
* **Patrón Strategy (Tarifas):** Interfaz `ITarifa` con variantes `TarifaFija`, `TarifaPorPeso` y `TarifaPorDistancia`.
* **Herencia y Polimorfismo (Modalidades):** Clase abstracta `ModalidadServicio` con subclases `ServicioEstandar` (72h), `ServicioExpres` (2h ruta + 24h entrega) y `ServicioProgramado` (franjas horarias).
* **Patrón Factory (Lectores):** Fábrica `CreadorLectores` para desacoplar la ingesta de archivos JSON y XML con codificación estricta `Encoding.UTF8`.
* **Resiliencia:** Tolerancia a fallos parciales ante datos corruptos (paquetes inválidos, guías duplicadas) auditados en `InformeInconsistencias` sin detener el procesamiento.
* **Exportación Polimórfica:** Generación de informes en JSON y XML bajo `InformesExportados/`.

### Cómo compilar y ejecutar el proyecto:

```bash
cd 04_Proyecto_Final/NexoLog
dotnet run
```

---

## 3. Sistema de Trabajo con Agentes de IA

Para la planeación, construcción y refactorización del código se utiliza un flujo colaborativo estipulado en [AGENTS.md](file:///c:/Users/pc/OneDrive/Documentos/GitHub/POO-3C/AGENTS.md):
- **Tutor / Monitor General:** Supervisa la coherencia técnica, la pedagogía y la capacidad de sustentación del estudiante.
- **Agente de Planeación (`poo-planner`):** Analiza requerimientos, desglosa responsabilidades (CRC) y diseña diagramas UML.
- **Agente de Construcción (`poo-builder`):** Implementa el código en C# respetando normas estrictas de cátedra.
- **Agente de Refactorización (`poo-refactor`):** Poda código, simplifica métodos extensos y elimina sobreingeniería.

---

## 4. Guía de Estilo Portátil

Las directrices completas de programación en C# (declaraciones individuales obligatorias, auto-propiedades limpias, constructores horizontales, eliminación de `switch` y `TimeSpan`, y nombres de dominio puro) se encuentran consolidadas en [REGLAS_PROGRAMACION.md](file:///c:/Users/pc/OneDrive/Documentos/GitHub/POO-3C/REGLAS_PROGRAMACION.md).
