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

## 2. Mapa de Clases y Carpetas

El código está dividido en tres capas ordenadas:

```text
04_Proyecto_Final/NexoLog/
├── Dominio/                              # Las entidades del negocio puro
│   ├── Paquete.cs                       # Cada bulto con peso (> 0) y valor (>= 0)
│   ├── Acontecimiento.cs                # Cada evento en la vida del envío (REC, RUT, ENT, DEV)
│   ├── Despacho.cs                      # El envío central con su lista de paquetes y tracking
│   ├── Establecimiento.cs               # El comercio remitente (COM-01, COM-02, COM-03)
│   ├── NexoLogException.cs              # Excepción propia para capturar errores de negocio
│   ├── Enums/ (TipoAcontecimiento, EstadoDespacho)
│   ├── Modalidades/                     # Jerarquía de entrega (Estándar, Exprés, Programado)
│   ├── Tarifas/                         # Estrategias de cobro (Fija, Peso, Distancia)
│   └── Inconsistencias/                 # Registro de auditoría para anomalías detectadas
│
├── Infraestructura/                      # Entrada y salida de datos
│   ├── Lectores/                        # Lectores de JSON y XML con codificación UTF-8
│   │   ├── LectorArchivoFactory.cs      # Fábrica para instanciar el lector adecuado
│   │   └── DTOs/                        # Modelos simples para recibir el texto de los archivos
│   └── Exportadores/                    # Exportadores a JSON y XML (IExportadorInforme)
│
├── Servicios/                           # Lógica de coordinación
│   ├── ServicioIntegracion.cs           # Junta pedidos, acuerdos y ordena el tracking
│   └── ServicioInformes.cs              # Calcula métricas operativas y financieras
│
└── Program.cs                           # Menú interactivo de consola (Opciones 1 a 6)
```

---

## 3. Los dos Patrones de Diseño (Cómo explicarlos al docente)

### Patrón Strategy (Estrategias de Cobro)
* **¿Dónde está?** En la interfaz `IEstrategiaCobro` y sus clases: `TarifaFijaStrategy`, `TarifaPesoStrategy`, `TarifaDistanciaStrategy`.
* **¿Por qué se usó?** Porque cada comercio cobra de manera diferente. En vez de llenar la clase `Despacho` o `Establecimiento` con `switch` o `if (tipo == "peso")`, cada estrategia encapsula su propia matemática. Si mañana NexoLog crea una tarifa mixta o por zonas, solo creamos una clase nueva sin tocar el resto del sistema.
* **Comprobación clave:** Para Comercio Caribe (`COM-01`) se aplica tarifa por peso ($3.500/kg). El despacho `NX-1001` pesa 3.7 kg y liquida exactamente **$12.950**, tal como lo pide el enunciado.

### Patrón Factory (Fábrica de Lectores)
* **¿Dónde está?** En `LectorArchivoFactory`.
* **¿Por qué se usó?** Centraliza la decisión de qué lector crear según la extensión (`.json` o `.xml`) y el tipo de dato. El resto del programa no necesita saber cómo se lee un XML o un JSON; solo le pide a la fábrica el lector y recibe los datos listos.

---

## 4. Reglas Clave para Defender en la Sustentación

1. **¿Por qué el estado inicial nunca es `null`?**
   El despacho nace explícitamente en `EstadoDespacho.Pendiente` en el constructor. Nunca existe un objeto en un estado indefinido.
2. **¿Por qué se ordenan los acontecimientos antes de procesarlos?**
   Los operadores logísticos envían archivos con eventos en desorden (por ejemplo, el evento de entrega aparece antes que la recepción en el texto). Si procesáramos en orden de llegada, el sistema rechazaría la entrega por "no haber salido a ruta". Ordenar por `FechaHora` ascendente primero reconstruye la línea de tiempo real.
3. **¿Cómo se calculan las tarifas?**
   Solo se cobran los despachos con estado `Entregado`. Los despachos devueltos o pendientes no generan cobro ($0). Si un despacho se entregó tarde, sigue siendo facturable según las reglas comerciales de la empresa.
4. **¿Cómo se calcula el porcentaje de cumplimiento (SLA)?**
   Se calcula **únicamente sobre los despachos entregados** ($\frac{\text{Cumplidos}}{\text{Entregados}} \times 100$). Los pendientes y devueltos no se cuentan en el denominador.
5. **¿Por qué el sistema no se cae ante errores en los archivos?**
   Aplica resiliencia: si un registro trae un peso de 0 kg o una guía repetida, no detiene el programa. Crea un `RegistroInconsistencia`, lo guarda en la auditoría y continúa procesando los despachos sanos.
