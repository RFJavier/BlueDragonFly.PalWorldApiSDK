using System.Text.Json.Serialization;

namespace BlueDragonFly.PalWorldApiSDK.Core;

/// <summary>Metricas de rendimiento y capacidad del servidor Palworld.</summary>
public sealed record ServerMetrics(
    [property: JsonPropertyName("serverfps")] int ServerFps,
    [property: JsonPropertyName("currentplayernum")] int CurrentPlayerCount,
    [property: JsonPropertyName("serverframetime")] double ServerFrameTimeMilliseconds,
    [property: JsonPropertyName("maxplayernum")] int MaxPlayerCount,
    [property: JsonPropertyName("uptime")] int UptimeSeconds,
    [property: JsonPropertyName("basecampnum")] int BaseCampCount,
    [property: JsonPropertyName("days")] int Days);
