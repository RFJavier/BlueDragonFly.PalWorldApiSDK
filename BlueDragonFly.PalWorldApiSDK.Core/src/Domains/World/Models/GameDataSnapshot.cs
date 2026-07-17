using System.Text.Json.Serialization;

namespace BlueDragonFly.PalWorldApiSDK.Core;

/// <summary>Instantanea del mundo devuelta por <c>GET /v1/api/game-data</c>.</summary>
public sealed record GameDataSnapshot(
    [property: JsonPropertyName("Time")] string Time,
    [property: JsonPropertyName("FPS")] float FramesPerSecond,
    [property: JsonPropertyName("AverageFPS")] float AverageFramesPerSecond,
    [property: JsonPropertyName("ActorData")] IReadOnlyList<GameActor> Actors);
