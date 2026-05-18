# MMI-SP
_Un mod para GTA V_
__Mors Mutual Insurance - Single Player__

### Basado en el trabajo original de Bob74 (2022)

[![Vídeo del tráiler](https://user-images.githubusercontent.com/9498543/162617439-42459c98-9915-4a43-b476-c339192e307a.png)](https://www.youtube.com/watch?v=WATdK3aOdGk)

¿Cansado de perder tu vehículo tuneado de $500,000 porque se te quedó por una misión o decidiste probar si era sumergible? ¡No esperes más y asegura tu vehículo ahora en Mors Mutual Insurance!

La descarga de la versión W.I.P. está disponible [aquí](https://github.com/JesusRondon2310/MMI-SP-Rewrite-WIP/releases/tag/0.3) 

# Historial de Cambios

### Alpha 0.3.4 (18/05/2026)
- **Refactorización final y Patrón de Coincidencia.** Se modularizó la lógica del asegurador, se mejoró el manejo de errores en el monitoreo de vehículos y se unificaron las funciones de fábrica.

### Alpha 0.3.3 (18/05/2026)
- **Corrección de errores críticos.** Se arregló un error que provocaba salir de la oficina al recuperar un vehículo, se estabilizó el estado "Destruido" y la actualización del submenú de recuperación.

### Alpha 0.3.2 (17/05/2026)
- **Módulo de Recuperación y su menú.** Se añadió la funcionalidad de reclamar vehículos destruidos a través de un nuevo módulo y submenú. El vehículo aparecerá en el depósito.

### Alpha 0.3.1 (17/05/2026)
- **Refactorización inicial y preparación.** Se reescribió gran parte de la lógica del seguro usando patrones funcionales para hacer el código más robusto y fácil de mantener.

### Alpha 0.3.0 (16/05/2026)
- **Persistencia Real.** ¡Tus coches sobreviven al cerrar y cargar la partida! Se añadió un sistema para guardar la ubicación exacta y recrear los vehículos asegurados al iniciar el juego.

### Alpha 0.2.3 (15/05/2026)
- Migración a `Result<T>` y pattern matching en todos los métodos críticos.
- Resueltos 15 errores de compilación por APIs obsoletas de SHVDN3 (`CS0618`).
- Base de datos JSON con Newtonsoft (`db.json`).

### Alpha 0.2.1 (14/05/2026)
- Refactorización inicial aplicando SOM (Sistema de Orquestación Modular).
- Eliminados archivos obsoletos (`InsuredVehicles.cs`, `Action.cs`, `Create.cs` duplicados).
- Creadas las fábricas `Buttons/Build.cs` y `Buttons/Fill.cs`.

### Alpha 0.2.0 (13/05/2026)
- Menú principal con los botones "Asegurar" y "Cancelar seguro".
- Solucionados bugs de salida de oficina, descripciones cortadas, orden de botones y `NullReferenceException` en NativeUI.

### Alpha 0.1.0 (12/05/2026)
- Migración del código original de Bob74 a SHVDN3.
- Entrada a la oficina y estructura básica del mod.

---

## Notas de Instalación
Recuerda dar los permisos de "Modificar" a tu usuario en la carpeta del juego para que ScriptHook y el mod puedan funcionar correctamente.

## Créditos
*   **Desarrollo y Arquitectura:** [JesusRondon2310](https://github.com/JesusRondon2310)
*   **Colaborador:** [Ricardo Vera](https://github.com/ricardovera76)
