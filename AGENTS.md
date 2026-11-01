# Definición y Manual de Agentes - Proyecto POO

Este repositorio cuenta con un sistema de trabajo colaborativo asistido por tres roles de Inteligencia Artificial para el desarrollo del Proyecto Final de Programación Orientada a Objetos (C#).

---

## 1. Arquitectura de Roles

| Rol | Identificador | Responsabilidad Principal |
| :--- | :--- | :--- |
| **Tutor / Monitor General** | *Agente Principal (Tú & Yo)* | Supervisa el proceso pedagógico, discute alternativas, valida que el código sea comprensible para el usuario y orquesta a los agentes subordinados. |
| **Agente de Planeación** | `poo-planner` | Analiza requerimientos, desglosa entidades y relaciones (UML), define validaciones y formula planes secuenciales paso a paso. |
| **Agente de Construcción** | `poo-builder` | Traduce los planes aprobados en código C# limpio, respetando las normas de estilo de clase (declaraciones explícitas, encapsulamiento y código defendible). |

---

## 2. Pautas Estrictas de Código para `poo-builder`

1. **Declaraciones explícitas e individuales:**
   ```csharp
   // Correcto (según preferencia de clase)
   double nota1;
   double nota2;
   double nota3;

   // Evitar
   double nota1, nota2, nota3;
   ```
2. **Encapsulamiento y POO académica:**
   - Atributos privados con propiedades públicas o getters/setters justificados.
   - Constructores claros que validen datos de entrada críticos.
   - Colecciones nativas (`List<T>`, arreglos) acordes a las sesiones del curso.
3. **Cero sobreingeniería:** No utilizar patrones complejos (Factory, Mediator, CQRS) a menos que el enunciado del proyecto o el docente lo exijan. Todo código debe ser fácil de explicar y sustentar en una sustentación oral.

---

## 3. Flujo de Trabajo

1. **Requerimiento:** Se presenta una fase o problema del enunciado.
2. **Planeación (`poo-planner`):** El planificador entrega el modelo conceptual (clases, métodos, atributos, relaciones) y una lista de tareas dividida en pasos lógicos.
3. **Revisión y Aprobación:** El usuario y el tutor revisan el plan, ajustan lo que no quede claro y dan luz verde.
4. **Construcción (`poo-builder`):** El constructor escribe el código C# del paso correspondiente en el proyecto y explica cómo probarlo.
5. **Verificación:** Se ejecuta y prueba el código, asegurando que el estudiante comprenda cada línea.
