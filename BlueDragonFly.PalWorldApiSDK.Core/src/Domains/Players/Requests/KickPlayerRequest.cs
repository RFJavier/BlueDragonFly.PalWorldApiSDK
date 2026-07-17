using System.Text.Json.Serialization;

namespace BlueDragonFly.PalWorldApiSDK.Core;

internal sealed record KickPlayerRequest(
    [property: JsonPropertyName("userid")] string SteamId,
    [property: JsonPropertyName("message")] string? Message);
