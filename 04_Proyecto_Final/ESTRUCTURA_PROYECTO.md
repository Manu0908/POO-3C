# NexoLog S.A.S. - Guía de Estructura y Sustentación

Este documento es una guía clara, sencilla y humana para comprender la estructura del proyecto y defender cada decisión técnica en la sustentación oral.

---

## 1. ¿Qué problema resuelve el sistema?

NexoLog coordina envíos entre comercios y transportadoras. El reto principal es que los datos llegan en formatos distintos (JSON y XML) y con errores deliberados (duplicados, fechas desordenadas, guías inexistentes y paquetes con pesos inválidos).

El software:
1. Lee todos los archivos sin caerse ante los errores (resiliencia).
2. Ordena los eventos en el tiempo y valida que la mercancía siga su ciclo natural (`Recibido` -> `En ruta` -> `Entregado` o `Devuelto`).
3. Liquida los cobros únicamente sobre los despachos entregados exitosamente.
4. Genera tres informes consolidados (Operativo, Financiero e Inconsistencias) y los exporta tanto en JSON como en XML.

---

## 2. Mapa de Clases Simplificado

```text
04_Proyecto_Final/NexoLog/
├── Despacho.cs                  # Envio central con maquina de estados (Pendiente, EnDistribucion, Entregado, Devuelto)
├── Paquete.cs                   # Cada bulto con peso (> 0) y valor declarado (>= 0)
├── Establecimiento.cs           # Comercio remitente (COM-01, COM-02, COM-03)
├── Acontecimiento.cs            # Hitos logisticos (REC, RUT, ENT, DEV)
├── RegistroInconsistencia.cs    # Registro de auditoria para anomalias detectadas
├── NexoLogException.cs          # Excepcion propia para capturar errores de reglas de negocio
├── Empresa.cs                   # Entidad coordinadora central (carga, integracion y generacion de informes)
├── Program.cs                   # Menu universitario interactivo en consola (Opciones 1 a 6)
│
├── Tarifas/                     # Estrategias de cobro polimorficas (Pattern Strategy)
│   ├── ITarifa.cs               # Contrato para el calculo de tarifas
│   ├── TarifaFija.cs            # Tarifa fija pactada por envio
│   ├── TarifaPorPeso.cs         # Tarifa por kilogramo acumulado ($3.500/kg en COM-01)
│   └── TarifaPorDistancia.cs    # Tarifa por kilometro recorrido
│
├── Servicios/                   # Modalidades de servicio y evaluacion de SLAs (Herencia)
│   ├── ModalidadServicio.cs     # Clase base abstracta de modalidad
│   ├── ServicioEstandar.cs      # Entrega en un maximo de 72 horas desde REC
│   ├── ServicioExpres.cs        # Salida <= 2h y entrega <= 24h desde REC
│   └── ServicioProgramado.cs    # Entrega en fecha y franja horaria pactada (horas como enteros simples)
│
├── Lectores/                    # Lectores de archivos JSON y XML con codificacion UTF-8
│   ├── ILectorArchivo.cs        # Contrato generico para lectura de archivos
│   ├── CreadorLectores.cs       # Creador centralizado de instancias (Pattern Factory)
│   ├── LectorAcuerdosJson.cs    # Lector de acuerdos comerciales
│   ├── LectorPedidosJson.cs     # Lector de pedidos en JSON
│   ├── LectorPedidosXml.cs      # Lector de pedidos en XML
│   ├── LectorSeguimientosJson.cs# Lector de seguimientos en JSON
│   └── LectorSeguimientosXml.cs # Lector de seguimientos en XML
│
├── DTOs/                        # Modelos simples para deserializacion de insumos
│   ├── AcuerdoDto.cs
│   ├── PedidoJsonDto.cs
│   ├── PaqueteJsonDto.cs
│   ├── AcontecimientoJsonDto.cs
│   └── LoteEstablecimientoDto.cs
│
├── Informes/                    # Modelos de datos para los informes consolidados
│   ├── InformeOperativo.cs      # Metricas operativas y desglose por comercio/modalidad
│   ├── InformeFinanciero.cs     # Liquidacion financiera por comercio y modalidad
│   └── InformeInconsistencias.cs# Consolidacion y agrupacion de irregularidades
│
└── Exportadores/                # Exportacion polimorfica en UTF-8
    ├── IExportadorInforme.cs    # Contrato comun de exportacion
    ├── ExportadorJson.cs        # Exportador a formato JSON estructurado
    └── ExportadorXml.cs         # Exportador a formato XML estructurado
```

---

## 3. Decisiones de Diseño Clave y Respuestas para el Docente

### ¿Por qué constantes `string` en vez de `enum`?
Se utilizaron constantes de texto (`public const string EstadoPendiente = "Pendiente";`) directamente dentro de `Despacho`, `Acontecimiento` y `RegistroInconsistencia`. Esta alternativa es estándar en C#, no requiere conceptos que aún no se hayan evaluado y previene errores tipográficos al referenciar `Despacho.EstadoPendiente`.

### ¿Qué es `Empresa`? (El concepto de Entidad Coordinadora en POO)
Representa a la entidad central del negocio (`Empresa.cs`). No representa un solo paquete ni un camión, sino a la empresa que coordina la carga de archivos, organiza los comercios y despachos en memoria y emite los reportes. Así se evita recargar al `Program.cs` o a las entidades individuales con tareas que no les corresponden.

### ¿Cómo se resolvió la máquina de estados en `Despacho`?
El método `ProcesarAcontecimiento` se fragmentó en 5 submétodos privados con nombres directos en español (`ValidarNoFinalizado`, `ProcesarRecepcion`, `ProcesarSalidaRuta`, `ProcesarEntrega`, `ProcesarDevolucion`), usando cadenas claras de `if / else if / else` en lugar de `switch`, logrando que cada método haga una sola cosa bien y sea fácil de explicar en un tablero.

### Patrón Strategy (Tarifas Comerciales)
* **¿Dónde está?** En la interfaz `ITarifa` y sus clases: `TarifaFija`, `TarifaPorPeso`, `TarifaPorDistancia`.
* **¿Por qué se usó?** Porque cada comercio cobra de manera diferente. En vez de llenar la clase `Despacho` o `Establecimiento` con condicionales por tipo de tarifa, cada estrategia encapsula su propia matemática mediante polimorfismo. Si mañana NexoLog crea una tarifa mixta o por zonas, solo creamos una clase nueva sin tocar el resto del sistema.
* **Comprobación clave:** Para Comercio Caribe (`COM-01`) se aplica tarifa por peso ($3.500/kg). El despacho `NX-1001` pesa 3.7 kg y liquida exactamente **$12.950**, tal como lo pide el enunciado.

### Patrón Factory (Creador de Lectores)
* **¿Dónde está?** En `CreadorLectores`.
* **¿Por qué se usó?** Centraliza la decisión de qué lector crear según la extensión (`.json` o `.xml`) y el tipo de dato. El resto del programa no necesita saber cómo se lee un XML o un JSON; solo le pide al creador el lector correspondiente y recibe los datos listos.

---

## 4. Preguntas Frecuentes en la Sustentación Oral

1. **¿Por qué el estado inicial nunca es `null`?**
   El despacho nace explícitamente en `Despacho.EstadoPendiente` en el constructor. Nunca existe un objeto en un estado indefinido.
2. **¿Por qué se ordenan los acontecimientos antes de procesarlos?**
   Los operadores logísticos envían archivos con eventos en desorden (por ejemplo, el evento de entrega aparece antes que la recepción en el archivo físico). Si procesáramos en orden de llegada, el sistema rechazaría la entrega por "no haber salido a ruta". Ordenar por `FechaHora` ascendente primero reconstruye la línea de tiempo real.
3. **¿Cómo se calculan las tarifas?**
   Solo se cobran los despachos con estado `Entregado`. Los despachos devueltos o pendientes no generan cobro ($0). Si un despacho se entregó tarde, sigue siendo facturable según las reglas comerciales de la empresa.
4. **¿Cómo se calcula el porcentaje de cumplimiento (SLA)?**
   Se calcula **únicamente sobre los despachos entregados** ($\frac{\text{Cumplidos}}{\text{Entregados}} \times 100$). Los pendientes y devueltos no se cuentan en el denominador.
5. **¿Por qué el sistema no se cae ante errores en los archivos?**
   Aplica resiliencia: si un registro trae un peso de 0 kg o una guía repetida, no detiene el programa. Crea un `RegistroInconsistencia`, lo guarda en la auditoría y continúa procesando los despachos sanos.
