# Datos de Prueba para NexoLog S.A.S.

Este directorio contiene los cinco (5) conjuntos de datos de prueba diseñados conforme a los requerimientos de las páginas 5 y 6 del documento rector `00_Proyecto Final POO 3C.pdf`.

---

## 1. Relación de Archivos Generados

| Archivo | Formato | Origen / Entidad | Propósito Principal |
| :--- | :--- | :--- | :--- |
| `acuerdos.json` | JSON | NexoLog Comercial | Tarifas y modalidades de cobro para COM-01, COM-02 y COM-03. |
| `pedidos_caribe.json` | JSON | Comercio Caribe (COM-01) y Comercio Sierra (COM-03) | Lotes de despachos estándar, exprés y programados, incluyendo NX-1001 e inconsistencias deliberadas. |
| `pedidos_andino.xml` | XML | Comercio Andino (COM-02) | Solicitudes en XML con paquetes, distancias, modalidades e inconsistencias (peso 0, duplicados). |
| `seguimiento_norte.json` | JSON | Transportadora Norte | Trazabilidad y acontecimientos (REC, RUT, ENT, DEV), orden desordenado, guía inexistente y validación de NX-1001. |
| `seguimiento_andina.xml` | XML | Transportadora Andina | Trazabilidad en XML con transiciones de estado e inconsistencia de secuencia (ENT sin previo RUT). |

---

## 2. Detalle de Casos de Negocio Clave

### A. Despacho Emblemático `NX-1001` (Páginas 5 y 6 del enunciado)
- **Comercio Remitente:** Comercio Caribe (`COM-01`).
- **Modalidad de Entrega:** Exprés (`EXPRES`).
- **Paquetes:** 2 paquetes (`PAQ-1001-A` con 2.2 kg y `PAQ-1001-B` con 1.5 kg).
- **Peso Total Acumulado:** $2.2 + 1.5 = 3.7\text{ kg}$.
- **Trazabilidad (Transportadora Norte):**
  - Recepción (`REC`): `2026-11-04 08:30:00`.
  - Salida a ruta (`RUT`): `2026-11-04 09:45:00` (1 h y 15 min tras recepción $\le$ 2 h $\rightarrow$ **Cumple salida exprés**).
  - Entrega final (`ENT`): `2026-11-04 18:15:00` (9 h y 45 min tras recepción $\le$ 24 h $\rightarrow$ **Cumple entrega exprés**).
  - *Nota:* En `seguimiento_norte.json`, estos eventos aparecen deliberadamente desordenados cronológicamente para exigir ordenamiento por fecha previo a la integración.
- **Liquidación Financiera:**
  - Modalidad de acuerdo para COM-01: **Tarifa por peso**.
  - Tarifa base: $\$3.500$ por kilogramo.
  - Cálculo: $3.7\text{ kg} \times \$3.500 = \mathbf{\$12.950}$ (concordancia exacta con el valor estipulado en la pág. 6).

---

## 3. Matriz de Modalidades y Acuerdos Comerciales

| Comercio | Código | Modalidad de Cobro | Valor Base | Justificación en la Simulación |
| :--- | :--- | :--- | :--- | :--- |
| **Comercio Caribe** | `COM-01` | **PESO** | $\$3.500\text{ / kg}$ | Aplica cobro proporcional al peso acumulado. Genera exactamente $\$12.950$ para NX-1001. |
| **Comercio Andino** | `COM-02` | **DISTANCIA** | $\$150\text{ / km}$ | Aplica cobro por kilómetro recorrido hacia los municipios boyacenses/andinos. |
| **Comercio Sierra** | `COM-03` | **FIJA** | $\$18.500\text{ fija}$ | Tarifa corporativa fija por despacho completado exitosamente. |

---

## 4. Banco de Inconsistencias Deliberadas para Pruebas (Fase 4 & 5)

| Tipo de Inconsistencia | Archivo | Registro Afectado | Comportamiento Esperado del Sistema |
| :--- | :--- | :--- | :--- |
| **Duplicado Exacto** | `pedidos_caribe.json` | `NX-1002` repetido | Se incorpora la primera ocurrencia y se omite la segunda sin duplicar registros ni fallar. |
| **Conflicto de Información** | `pedidos_caribe.json` | `NX-1001` con destino/destinatario diferente | Se detecta conflicto de guía existente, se preserva el original y se reporta en el informe de inconsistencias. |
| **Peso Inválido ($\le 0$)** | `pedidos_caribe.json` / `pedidos_andino.xml` | `NX-1090` (0 kg) / `NX-2090` (0 kg) | Rechazo del paquete/despacho por violar la regla de peso estrictamente positivo. |
| **Despacho sin Paquetes** | `pedidos_caribe.json` / `pedidos_andino.xml` | `NX-1091` / `NX-2092` | Rechazo por infringir la regla que exige al menos un paquete por guía. |
| **Horario Programado Invertido** | `pedidos_caribe.json` | `NX-3090` (17:00 a 13:00) | Rechazo por intervalo inválido (hora fin anterior a hora inicio). |
| **ID de Paquete Duplicado** | `pedidos_caribe.json` | `NX-3091` (`PAQ-DUP-01`) | Rechazo por identificador repetido dentro de la misma guía. |
| **Valor Declarado Negativo** | `pedidos_andino.xml` | `NX-2091` (-$25.000) | Rechazo por regla de valor no negativo. |
| **Referencia a Guía Inexistente** | `seguimiento_norte.json` / `seguimiento_andina.xml` | `NX-9999` / `NX-7777` | Reporte en informe de inconsistencias al no hallarse la guía en los pedidos. |
| **Secuencia Ilegal (ENT sin RUT)** | `seguimiento_andina.xml` | `NX-2004` (Pasa de REC a ENT) | Rechazo del acontecimiento por violar el flujo lógico (la mercancía no puede entregarse sin salir a ruta). |
| **Evento posterior a Finalización** | `seguimiento_norte.json` / `seguimiento_andina.xml` | `NX-1001` / `NX-2001` | No se admiten eventos que pretendan reactivar o alterar un despacho ya finalizado. |
