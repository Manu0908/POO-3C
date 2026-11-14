# POO-3C: Programación y Diseño Orientado a Objetos

Repositorio académico de la asignatura **Programación Orientada a Objetos (C# / .NET 10)** correspondiente al **Tercer Cuatrimestre de Ingeniería en Ciencia de Datos** (Universidad del Magdalena).

El propósito central de este repositorio es construir software limpio, modular, progresivo y 100% defendible en sustentaciones orales.

---

## 1. Estructura General del Repositorio

```text
POO-3C-IA/
├── [Documentos Rectores]
│   ├── 00_MicroDiseño - ICD.pdf
│   ├── 00_Cronograma Detallado de Clases.pdf
│   └── 00_Proyecto Final POO 3C.pdf
│
├── [Diapositivas de Clase]
│   ├── Sesion_01 - Introduccion a POO.pdf
│   ├── Sesion_02 - Colecciones en C#.pdf
│   ├── Sesion_03 - Clases, Objetos y Abstraccion.pdf
│   ├── Sesion_05 - Encapsulamiento.pdf
│   ├── Sesion_07 - Generalidades al construir una clase.pdf
│   ├── Sesion_09 - Herencia.pdf
│   ├── Sesion_10 - Relaciones y colaboraciones entre objetos.pdf
│   ├── Sesion_12 - Polimorfismo.pdf
│   ├── Sesion_14 - Clases Abstractas, Interfaces y Excepciones.pdf
│   ├── Sesion_15 - UML.pdf
│   ├── Sesion_17 - Serializacion + DTO + Deserealizacion.pdf
│   └── Sesion_19 - Strategy + Factory Method.pdf
│
├── 01_Ejemplos_Clase/
│   ├── Sesion_03_Gimnasio/
│   ├── Sesion_05_Encapsulamiento_Cine/
│   ├── Sesion_07_Vehiculo/
│   ├── Sesion_10_Herencia_Empleados/
│   ├── Sesion_12_Polimorfismo_Hotel/
│   └── Sesion_19_Parciales_Finales_Y_Ejemplo_Clase/
│
├── 02_Actividades_y_Talleres/
│   ├── Sesion_04_Taller_Cine_CRC/
│   ├── Sesion_06_Actividad_MoviCar/
│   ├── Sesion_13_Actividad_ClasesAbstractas/
│   ├── Sesion_15_Taller_Veterinaria_UML/
│   ├── Sesion_16_Taller_Alquiler_Vehiculos/
│   └── Practica Segundo Parcial/
│
├── 03_Evaluaciones/
│   ├── Sesion_08_Parcial_01/
│   └── Sesion_11_Recuperacion_Parcial_01_EcoRide/
│
├── 04_Proyecto_Final/
│   ├── NexoLog/
│   ├── Diagrama_UML_NexoLog.drawio
│   └── Diagrama_UML_NexoLog.drawio.pdf
│
├── InformesExportados/
├── Guia_Arquitectura_NexoLog.pdf
└── AGENTS.md
```

---

## 2. Proyecto Final: NexoLog S.A.S.

Sistema de consola en C# / .NET 10 para la integración y análisis resiliente de operaciones logísticas multifuente:

* **Entidades de Dominio Puro:** `Empresa`, `Establecimiento`, `Despacho`, `Paquete`, `Acontecimiento`, `RegistroInconsistencia`.
* **DTOs y Mappers (Sesión 17):** Capa `DTOs/` para deserialización desacoplada (`MapToDomain`) y exportación de reportes sin exponer el dominio (`InconsistenciaDto`, `InformeConsolidadoDto` con `MapToDto`).
* **Patrón Strategy (Tarifas e I/O):** Interfaz `ITarifa` (`ValorBase` polimórfico y `CalcularTarifa`) con `TarifaFija`, `TarifaPorPeso` y `TarifaPorDistancia`; además contratos simétricos `IImportadorArchivo<T>` e `IExportadorInforme`.
* **Herencia y Polimorfismo (Modalidades SLA):** Clase abstracta `ModalidadServicio` con `ServicioEstandar`, `ServicioExpres` y `ServicioProgramado`.
* **Patrón Factory Method:** Fábricas simétricas `CreadorImportadores` y `CreadorExportadores` para instanciar lectores y exportadores JSON/XML con codificación estricta `Encoding.UTF8`.
* **Informes y Resiliencia:** Tolerancia a fallos parciales ante insumos corruptos consolidados en `InformeOperativo`, `InformeFinanciero` e `InformeInconsistencias`, exportados bajo `InformesExportados/`.

### Compilación y ejecución

```bash
dotnet build 04_Proyecto_Final/NexoLog/NexoLog.csproj
dotnet run --project 04_Proyecto_Final/NexoLog/NexoLog.csproj
```

---

## 3. Reglas de Cátedra y Arquitectura

Las reglas completas de estilo C#, constructores vacíos, nomenclatura expresiva, DTOs, modelado UML en Draw.io y especificaciones del proyecto residen de forma unificada en [`AGENTS.md`](./AGENTS.md).
