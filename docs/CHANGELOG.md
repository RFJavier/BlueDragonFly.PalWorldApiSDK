# Changelog

Todos los cambios relevantes de este proyecto se documentan en este archivo.

El formato sigue [Keep a Changelog](https://keepachangelog.com/es-ES/1.1.0/) y el versionado previsto sigue [Semantic Versioning](https://semver.org/lang/es/).

## [1.0.0] - 2026-07-17

### Added

- Cliente `PalworldClient` organizado por dominios (`Server`, `World`, `Players`) para la API REST administrativa de Palworld.
- Configuracion en memoria mediante `PalworldServerOptions`, con normalizacion automatica de la URL a `/v1/api`.
- Autenticacion HTTP Basic, encabezados JSON y manejo de excepciones centralizados en la capa HTTP interna.
- Modelos tipados: `ServerInfo`, `PlayerList`, `PlayerInfo`, `ServerMetrics`, `ServerSettings`, `GameDataSnapshot`, `CharacterActor`, `PalBoxActor` y `UnknownGameActor`.
- Convertidor polimorfico `GameActorJsonConverter` para actores de `/game-data`, tolerando tipos futuros no modelados.
- Excepciones tipadas: `PalworldApiException`, `PalworldBadRequestException` y `PalworldUnauthorizedException`.
- Cobertura completa de los endpoints REST oficiales documentados: informacion, jugadores, metricas, configuracion, actores del mundo, anuncios, expulsiones, bloqueos, desbloqueos, guardado, apagado ordenado y detencion forzada.
- Aplicacion de consola `BlueDragonFLy.PalWorldApiSDK.Demo` con comandos por variables de entorno o archivo `.env`, incluyendo `help`.
- Plantilla `.env.example` para facilitar la configuracion local sin exponer credenciales en el repositorio.
- Pruebas unitarias para metodo HTTP, ruta, autenticacion, cuerpo JSON, deserializacion, manejo de errores y validacion de parametros.
- Documentacion: `README.md`, `docs/ARCHITECTURE.md`, `docs/ENDPOINTS.md`, `docs/USAGE.md`, `docs/PLATFORM.md` y `CHANGELOG.md`.
- Licencia MIT y metadatos completos de NuGet en el proyecto Core.
