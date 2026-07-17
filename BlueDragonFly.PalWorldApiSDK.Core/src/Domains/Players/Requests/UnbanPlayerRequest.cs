using System.Text.Json.Serialization;

namespace BlueDragonFly.PalWorldApiSDK.Core;

internal sealed record UnbanPlayerRequest([property: JsonPropertyName("userid")] string SteamId);
