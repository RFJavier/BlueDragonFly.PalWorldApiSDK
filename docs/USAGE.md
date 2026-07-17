# Guia de uso de BlueDragonFly.PalWorldApiSDK.Core

Esta guia explica paso a paso como usar el SDK para administrar un servidor Palworld desde .NET.

## Contenido

- [Requisitos previos](#requisitos-previos)
- [Instalar el paquete](#instalar-el-paquete)
- [Configurar la conexion](#configurar-la-conexion)
- [Crear el cliente](#crear-el-cliente)
- [Uso por dominios](#uso-por-dominios)
  - [Servidor](#servidor)
  - [Jugadores](#jugadores)
  - [Mundo](#mundo)
- [Modelos de respuesta](#modelos-de-respuesta)
- [Manejo de errores](#manejo-de-errores)
- [Cancelacion de operaciones](#cancelacion-de-operaciones)
- [Uso con inyeccion de dependencias](#uso-con-inyeccion-de-dependencias)
- [Demo](#demo)

## Requisitos previos

- Un proyecto .NET 10 o superior. Consulta [docs/PLATFORM.md](PLATFORM.md) si necesitas comprobar tu entorno.
- Un servidor Palworld con la API REST habilitada (`RESTAPIEnabled=True` en `PalWorldSettings.ini`).
- La contrasena de administrador configurada en el servidor.

## Instalar el paquete

Desde NuGet.org:

```powershell
dotnet add package BlueDragonFly.PalWorldApiSDK.Core
```

O con una version concreta:

```powershell
dotnet add package BlueDragonFly.PalWorldApiSDK.Core --version 1.0.0
```

## Configurar la conexion

Toda la configuracion se encapsula en `PalworldServerOptions`:

```csharp
var options = new PalworldServerOptions(
    baseUrl: "http://localhost:8212",
    username: "admin",
    password: "tu-contrasena");
```

- `baseUrl`: puede ser la URL del servidor (`http://localhost:8212`) o la URL completa de la API (`http://localhost:8212/v1/api`). El SDK normaliza la ruta a `/v1/api` automaticamente.
- `username`: usuario REST. Palworld suele usar `admin`.
- `password`: la contrasena de administrador del servidor.

La configuracion solo vive en memoria. Tu aplicacion puede obtener estos valores desde `appsettings.json`, secretos de usuario o variables de entorno, pero el SDK no lee ningun origen de configuracion por si mismo.

## Crear el cliente

```csharp
using BlueDragonFLy.PalWorldApiSDK.Core;

using var client = new PalworldClient(options);
```

`PalworldClient` expone tres dominios:

- `client.Server` — informacion, metricas, configuracion y apagado.
- `client.Players` — listado, expulsion, bloqueo y desbloqueo.
- `client.World` — anuncios, guardado e instantanea de actores.

Si tu aplicacion ya gestiona su propio `HttpClient` (por ejemplo, con `IHttpClientFactory`), usa el constructor que lo recibe:

```csharp
using var client = new PalworldClient(options, httpClient);
```

> En este segundo caso el SDK **no** libera el `HttpClient`; tu aplicacion sigue siendo responsable de su ciclo de vida.

## Uso por dominios

### Servidor

```csharp
// Informacion general
var info = await client.Server.InfoAsync();
Console.WriteLine($"{info.ServerName} v{info.Version} - {info.WorldGuid}");

// Metricas de rendimiento
var metrics = await client.Server.MetricsAsync();
Console.WriteLine($"FPS: {metrics.ServerFps}; jugadores: {metrics.CurrentPlayerCount}/{metrics.MaxPlayerCount}");

// Configuracion activa
var settings = await client.Server.SettingsAsync();
Console.WriteLine($"Dificultad: {settings.Difficulty}; PvP: {settings.BIsPvP}");

// Apagado ordenado en 30 segundos
await client.Server.ShutdownAsync(30, "El servidor se reiniciara pronto.");

// Detencion inmediata (usar solo si shutdown no es posible)
await client.Server.StopAsync();
```

### Jugadores

```csharp
// Listar jugadores conectados
var players = await client.Players.ListAsync();
foreach (var player in players.Players)
{
    Console.WriteLine($"{player.Name} | {player.SteamId} | nivel {player.Level}");
}

// Administrar un jugador
var steamId = "steam_00000000000000000";

await client.Players.KickAsync(steamId, "Conexion finalizada.");
await client.Players.BanAsync(steamId, "Incumplimiento de reglas.");
await client.Players.UnbanAsync(steamId);
```

Importante: las operaciones de `KickAsync`, `BanAsync` y `UnbanAsync` usan el identificador Steam (`userid` en la API), no el `PlayerId` interno de Palworld. El formato esperado es `steam_XXXXXXXXXXXXXXX`.

### Mundo

```csharp
// Enviar un anuncio a todos los jugadores
await client.World.AnnounceAsync("Evento especial en 5 minutos.");

// Guardar el mundo
await client.World.SaveAsync();

// Obtener actores del mundo (puede devolver 404 si el servidor no lo soporta)
try
{
    var snapshot = await client.World.GameDataAsync();
    Console.WriteLine($"Actores: {snapshot.Actors.Count}; FPS: {snapshot.FramesPerSecond}");
}
catch (PalworldApiException exception) when (exception.StatusCode == System.Net.HttpStatusCode.NotFound)
{
    Console.WriteLine("Este servidor no expone /v1/api/game-data.");
}
```

## Modelos de respuesta

### `ServerInfo`

| Propiedad | JSON | Descripcion |
| --- | --- | --- |
| `Version` | `version` | Version del servidor. |
| `ServerName` | `servername` | Nombre publico del servidor. |
| `Description` | `description` | Descripcion del servidor. |
| `WorldGuid` | `worldguid` | GUID del mundo. |

### `ServerMetrics`

| Propiedad | JSON | Descripcion |
| --- | --- | --- |
| `ServerFps` | `serverfps` | FPS del servidor. |
| `CurrentPlayerCount` | `currentplayernum` | Jugadores conectados. |
| `MaxPlayerCount` | `maxplayernum` | Maximo de jugadores permitidos. |
| `ServerFrameTimeMilliseconds` | `serverframetime` | Tiempo de frame en milisegundos. |
| `UptimeSeconds` | `uptime` | Tiempo activo en segundos. |
| `BaseCampCount` | `basecampnum` | Numero de bases. |
| `Days` | `days` | Dias transcurridos en el mundo. |

### `ServerSettings`

Contiene casi todos los ajustes de `PalWorldSettings.ini` como propiedades nullable. Si Pocketpair agrega nuevos campos, apareceran en `AdditionalSettings` (`Dictionary<string, JsonElement>`) para que tu aplicacion no falle al actualizar el servidor.

### `PlayerInfo`

| Propiedad | JSON | Descripcion |
| --- | --- | --- |
| `Name` | `name` | Nombre del jugador. |
| `AccountName` | `accountName` | Cuenta asociada. |
| `PlayerId` | `playerId` | Identificador interno de Palworld. |
| `SteamId` | `userId` | Identidad Steam (`steam_...`). |
| `IpAddress` | `ip` | Direccion IP. |
| `Ping` | `ping` | Latencia. |
| `LocationX` / `LocationY` | `location_x` / `location_y` | Coordenadas. |
| `Level` | `level` | Nivel del jugador. |
| `BuildingCount` | `building_count` | Construcciones del jugador. |

### `GameDataSnapshot`

| Propiedad | JSON | Descripcion |
| --- | --- | --- |
| `Time` | `Time` | Tiempo de la instantanea. |
| `FramesPerSecond` | `FPS` | FPS actual. |
| `AverageFramesPerSecond` | `AverageFPS` | Promedio de FPS. |
| `Actors` | `ActorData` | Lista de actores (`CharacterActor`, `PalBoxActor` o `UnknownGameActor`). |

## Manejo de errores

Todas las respuestas no exitosas lanzan `PalworldApiException`, que expone:

- `StatusCode`: el codigo HTTP.
- `ResponseContent`: el cuerpo de respuesta del servidor.

Existen dos especializaciones:

- `PalworldBadRequestException` para errores `400`.
- `PalworldUnauthorizedException` para errores `401`.

Ejemplo:

```csharp
try
{
    await client.World.SaveAsync();
}
catch (PalworldUnauthorizedException)
{
    Console.WriteLine("Usuario o contrasena incorrectos.");
}
catch (PalworldBadRequestException exception)
{
    Console.WriteLine($"Solicitud incorrecta: {exception.ResponseContent}");
}
catch (PalworldApiException exception)
{
    Console.WriteLine($"Error {(int)exception.StatusCode}: {exception.ResponseContent}");
}
```

## Cancelacion de operaciones

Cualquier metodo acepta un `CancellationToken` opcional:

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
var info = await client.Server.InfoAsync(cts.Token);
```

## Uso con inyeccion de dependencias

```csharp
builder.Services.AddSingleton<PalworldClient>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var options = new PalworldServerOptions(
        configuration["Palworld:ServerUrl"]!,
        configuration["Palworld:Username"]!,
        configuration["Palworld:Password"]!);

    return new PalworldClient(options);
});
```

Tambien puedes registrar `PalworldServerOptions` como singleton y reutilizarlo para crear clientes de forma transitoria.

## Demo

El proyecto `BlueDragonFLy.PalWorldApiSDK.Demo` te permite probar cada operacion sin escribir codigo. Puedes configurarlo mediante variables de entorno o un archivo `.env`:

```powershell
# Opcion 1: variables de entorno
$env:PALWORLD_SERVER_URL="http://localhost:8212"
$env:PALWORLD_ADMIN_PASSWORD="tu-contrasena"
$env:PALWORLD_DEMO_COMMAND="info"
dotnet run --project BlueDragonFLy.PalWorldApiSDK.Demo

# Opcion 2: archivo .env
cp BlueDragonFLy.PalWorldApiSDK.Demo/.env.example BlueDragonFLy.PalWorldApiSDK.Demo/.env
# edita .env con tus valores reales
dotnet run --project BlueDragonFLy.PalWorldApiSDK.Demo
```

La demo busca `.env` primero en el directorio de trabajo y luego junto al ejecutable. Las variables de entorno reales tienen prioridad sobre el archivo `.env`.

| Variable | Requerida | Descripcion |
| --- | --- | --- |
| `PALWORLD_SERVER_URL` | Si | URL del servidor, por ejemplo `http://localhost:8212`. |
| `PALWORLD_ADMIN_PASSWORD` | Si | Contrasena del usuario REST. |
| `PALWORLD_ADMIN_USERNAME` | No | Usuario REST; el valor predeterminado es `admin`. |
| `PALWORLD_DEMO_COMMAND` | Si | Operacion: `info`, `players`, `metrics`, `settings`, `game-data`, `announce`, `kick`, `ban`, `unban`, `save`, `shutdown`, `stop` o `help`. |
| `PALWORLD_STEAM_ID` | Para `kick`, `ban`, `unban` | Identificador Steam enviado como `userid` a Palworld. |
| `PALWORLD_MESSAGE` | Para mensajes | Texto del anuncio, motivo o aviso de apagado. |
| `PALWORLD_SHUTDOWN_WAIT_SECONDS` | Para `shutdown` | Segundos de espera; valor predeterminado `30`. |

Usa `PALWORLD_DEMO_COMMAND=help` para imprimir la ayuda sin conectarte al servidor.

