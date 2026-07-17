using System.Text.Json.Serialization;

namespace BlueDragonFly.PalWorldApiSDK.Core;

internal sealed record BanPlayerRequest(
    [property: JsonPropertyName("userid")] string SteamId,
    [property: JsonPropertyName("message")] string? Message);
