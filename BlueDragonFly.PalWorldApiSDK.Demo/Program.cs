using BlueDragonFly.PalWorldApiSDK.Core;

LoadEnvFile();

var command = Environment.GetEnvironmentVariable("PALWORLD_DEMO_COMMAND")?.Trim().ToLowerInvariant() ?? "info";
if (string.IsNullOrWhiteSpace(command))
{
    Console.WriteLine("Define PALWORLD_DEMO_COMMAND. Usa 'help' para ver los comandos disponibles.");
    return;
}

if (command == "help")
{
    PrintHelp();
    return;
}

var serverUrl = RequireEnvironmentVariable("PALWORLD_SERVER_URL");
var username = Environment.GetEnvironmentVariable("PALWORLD_ADMIN_USERNAME") ?? "admin";
var password = RequireEnvironmentVariable("PALWORLD_ADMIN_PASSWORD");
using var client = new PalworldClient(new PalworldServerOptions(serverUrl, username, password));

try
{
    switch (command)
    {
        case "info":
            var info = await client.Server.InfoAsync();
            Console.WriteLine($"{info.ServerName} ({info.Version}) - {info.Description} - [{info.WorldGuid}]");
            break;
        case "players":
            var players = await client.Players.ListAsync();
            foreach (var player in players.Players)
            {
                Console.WriteLine($"{player.Name} | {player.SteamId} | nivel {player.Level}");
            }
            break;
        case "metrics":
            var metrics = await client.Server.MetricsAsync();
            Console.WriteLine($"FPS: {metrics.ServerFps}; jugadores: {metrics.CurrentPlayerCount}/{metrics.MaxPlayerCount}; uptime: {metrics.UptimeSeconds}s");
            break;
        case "settings":
            var settings = await client.Server.SettingsAsync();
            Console.WriteLine($"{settings.ServerName} | dificultad: {settings.Difficulty} | PvP: {settings.BIsPvP}");
            break;
        case "game-data":
            var snapshot = await client.World.GameDataAsync();
            Console.WriteLine($"{snapshot.Time} | FPS: {snapshot.FramesPerSecond} | actores: {snapshot.Actors.Count}");
            break;
        case "announce":
            await client.World.AnnounceAsync(RequireEnvironmentVariable("PALWORLD_MESSAGE"));
            break;
        case "kick":
            await client.Players.KickAsync(RequireEnvironmentVariable("PALWORLD_STEAM_ID"), Environment.GetEnvironmentVariable("PALWORLD_MESSAGE"));
            break;
        case "ban":
            await client.Players.BanAsync(RequireEnvironmentVariable("PALWORLD_STEAM_ID"), Environment.GetEnvironmentVariable("PALWORLD_MESSAGE"));
            break;
        case "unban":
            await client.Players.UnbanAsync(RequireEnvironmentVariable("PALWORLD_STEAM_ID"));
            break;
        case "save":
            await client.World.SaveAsync();
            break;
        case "shutdown":
            await client.Server.ShutdownAsync(GetShutdownWaitSeconds(), Environment.GetEnvironmentVariable("PALWORLD_MESSAGE"));
            break;
        case "stop":
            await client.Server.StopAsync();
            break;
        default:
            Console.Error.WriteLine($"Comando no reconocido: {command}. Usa 'help' para ver los comandos disponibles.");
            Environment.ExitCode = 1;
            return;
    }
}
catch (PalworldApiException exception) when (command == "game-data" && exception.StatusCode == System.Net.HttpStatusCode.NotFound)
{
    Console.Error.WriteLine("El servidor no expone /v1/api/game-data. Este endpoint requiere una version compatible de la API REST de Palworld.");
    Environment.ExitCode = 1;
}
catch (PalworldApiException exception)
{
    Console.Error.WriteLine($"Error de API ({(int)exception.StatusCode}): {exception.ResponseContent}");
    Environment.ExitCode = 1;
}

static string RequireEnvironmentVariable(string name) =>
    Environment.GetEnvironmentVariable(name) is { Length: > 0 } value
        ? value
        : throw new InvalidOperationException($"Define la variable de entorno {name}.");

static int GetShutdownWaitSeconds() =>
    int.TryParse(Environment.GetEnvironmentVariable("PALWORLD_SHUTDOWN_WAIT_SECONDS"), out var waitSeconds) && waitSeconds >= 0
        ? waitSeconds
        : 30;

static void LoadEnvFile()
{
    var candidates = new[]
    {
        Path.Combine(Directory.GetCurrentDirectory(), ".env"),
        Path.Combine(AppContext.BaseDirectory, ".env")
    };

    foreach (var path in candidates)
    {
        if (!File.Exists(path))
        {
            continue;
        }

        foreach (var rawLine in File.ReadLines(path))
        {
            var line = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#') || line.StartsWith("//"))
            {
                continue;
            }

            var separator = line.IndexOf('=');
            if (separator <= 0)
            {
                continue;
            }

            var key = line[..separator].Trim();
            var value = line[(separator + 1)..].Trim();

            if (value.Length >= 2 &&
                ((value.StartsWith('"') && value.EndsWith('"')) ||
                 (value.StartsWith('\'') && value.EndsWith('\''))))
            {
                value = value[1..^1];
            }

            // Las variables de entorno reales tienen prioridad sobre el archivo .env.
            if (Environment.GetEnvironmentVariable(key) is null)
            {
                Environment.SetEnvironmentVariable(key, value);
            }
        }

        break;
    }
}

static void PrintHelp()
{
    Console.WriteLine("Comandos disponibles:");
    Console.WriteLine("  info          Muestra informacion del servidor.");
    Console.WriteLine("  players       Lista los jugadores conectados.");
    Console.WriteLine("  metrics       Muestra metricas de rendimiento.");
    Console.WriteLine("  settings      Muestra la configuracion activa del servidor.");
    Console.WriteLine("  game-data     Muestra una instantanea de los actores del mundo.");
    Console.WriteLine("  announce      Publica un anuncio (requiere PALWORLD_MESSAGE).");
    Console.WriteLine("  kick          Expulsa a un jugador (requiere PALWORLD_STEAM_ID).");
    Console.WriteLine("  ban           Bloquea a un jugador (requiere PALWORLD_STEAM_ID).");
    Console.WriteLine("  unban         Elimina el bloqueo de un jugador (requiere PALWORLD_STEAM_ID).");
    Console.WriteLine("  save          Solicita guardar el mundo.");
    Console.WriteLine("  shutdown      Programa el apagado ordenado (requiere PALWORLD_SHUTDOWN_WAIT_SECONDS o usa 30s).");
    Console.WriteLine("  stop          Fuerza la detencion inmediata del servidor.");
    Console.WriteLine();
    Console.WriteLine("Variables de entorno:");
    Console.WriteLine("  PALWORLD_SERVER_URL            URL del servidor (requerida).");
    Console.WriteLine("  PALWORLD_ADMIN_PASSWORD        Contrasena del usuario REST (requerida).");
    Console.WriteLine("  PALWORLD_ADMIN_USERNAME        Usuario REST; predeterminado 'admin'.");
    Console.WriteLine("  PALWORLD_DEMO_COMMAND          Operacion a ejecutar (requerida).");
    Console.WriteLine("  PALWORLD_STEAM_ID              Identificador Steam para kick/ban/unban.");
    Console.WriteLine("  PALWORLD_MESSAGE               Texto del anuncio, motivo o aviso de apagado.");
    Console.WriteLine("  PALWORLD_SHUTDOWN_WAIT_SECONDS Segundos de espera para shutdown; predeterminado 30.");
    Console.WriteLine();
    Console.WriteLine("Tambien puedes definir estas variables en un archivo .env al lado del ejecutable.");
    Console.WriteLine("Las variables reales del sistema tienen prioridad sobre las del archivo .env.");
}
