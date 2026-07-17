# BlueDragonFly Palworld API SDK

[![NuGet](https://img.shields.io/nuget/v/BlueDragonFly.PalWorldApiSDK.Core.svg)](https://www.nuget.org/packages/BlueDragonFly.PalWorldApiSDK.Core/)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4.svg)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

Biblioteca .NET para administrar un servidor Palworld mediante su [API REST oficial](https://docs.palworldgame.com/category/rest-api/).

## Documentacion

- [Arquitectura](docs/ARCHITECTURE.md)
- [Cobertura de endpoints](docs/ENDPOINTS.md)
- [Guia de uso](docs/USAGE.md)
- [Plataforma de destino y compatibilidad](docs/PLATFORM.md)
- [Changelog](docs/CHANGELOG.md)

## Requisitos

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4.svg)](https://dotnet.microsoft.com/)

## Instalacion

```powershell
dotnet add package BlueDragonFly.PalWorldApiSDK.Core
```

## Uso rapido

```csharp
using BlueDragonFLy.PalWorldApiSDK.Core;

using var client = new PalworldClient(
    new PalworldServerOptions("http://localhost:8212", "admin", "your-password"));

var server = await client.Server.InfoAsync();
await client.World.AnnounceAsync("El servidor se reiniciara pronto.");
await client.Players.BanAsync("steam_00000000000000000", "Incumplimiento de las reglas.");
```

`PalworldServerOptions` acepta tanto la URL del servidor como la URL completa de la API y normaliza la ruta a `/v1/api`. El cliente conserva esta configuracion unicamente en memoria. Para persistirla, el proyecto consumidor puede cargar esos valores desde su propia configuracion JSON o variables de entorno.

## Operaciones incluidas

| Area | Metodos |
| --- | --- |
| Servidor | `Server.InfoAsync`, `Server.MetricsAsync`, `Server.SettingsAsync`, `Server.ShutdownAsync`, `Server.StopAsync` |
| Mundo | `World.AnnounceAsync`, `World.SaveAsync`, `World.GameDataAsync` |
| Jugadores | `Players.ListAsync`, `Players.KickAsync`, `Players.BanAsync`, `Players.UnbanAsync` |

Consulta la [guia de uso](docs/USAGE.md) para la tabla completa de variables, ejemplos de cada operacion y manejo de errores.

## Demo

El proyecto `BlueDragonFLy.PalWorldApiSDK.Demo` es una aplicacion de consola que expone todos los metodos del SDK mediante variables de entorno o un archivo `.env`:

```powershell
# Opcion 1: variables de entorno
$env:PALWORLD_SERVER_URL="http://localhost:8212"
$env:PALWORLD_ADMIN_PASSWORD="your-password"
$env:PALWORLD_DEMO_COMMAND="info"
dotnet run --project BlueDragonFLy.PalWorldApiSDK.Demo

# Opcion 2: archivo .env
cp BlueDragonFLy.PalWorldApiSDK.Demo/.env.example BlueDragonFLy.PalWorldApiSDK.Demo/.env
# edita .env con tus valores
dotnet run --project BlueDragonFLy.PalWorldApiSDK.Demo
```

Ejecuta con `PALWORLD_DEMO_COMMAND=help` para ver la lista completa de comandos. Ver [USAGE.md](docs/USAGE.md#demo) para los detalles de cada variable.

## Tests

```powershell
dotnet test
```

Las pruebas verifican la composicion de cada solicitud (metodo HTTP, ruta, autenticacion y cuerpo JSON), la deserializacion de respuestas oficiales, el manejo de errores HTTP y la validacion de parametros. No requieren un servidor real: usan un `HttpMessageHandler` interno.

## Compatibilidad

- **.NET 10** o superior. Consulta [docs/PLATFORM.md](docs/PLATFORM.md) para mas detalles sobre la plataforma de destino.
- Servidor Palworld con la API REST habilitada (`RESTAPIEnabled=True`).
- El endpoint `/v1/api/game-data` puede no estar disponible en versiones antiguas del servidor.

## Seguridad

La API de Palworld usa Basic Auth y permite acciones administrativas destructivas. Habilita `RESTAPIEnabled=True` en el servidor y no expongas este endpoint directamente a Internet. Se recomienda usar secretos del entorno, una red privada o un proxy con controles de acceso adicionales.

## Licencia

Este proyecto se distribuye bajo la licencia [MIT](LICENSE).
Copyright (c) 2026 Javier Rivera.
