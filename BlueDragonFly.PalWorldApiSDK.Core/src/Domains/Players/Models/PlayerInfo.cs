using System.Text.Json.Serialization;

namespace BlueDragonFly.PalWorldApiSDK.Core;

/// <summary>Datos de un jugador conectado o registrado por el servidor.</summary>
public sealed record PlayerInfo(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("accountName")] string? AccountName,
    [property: JsonPropertyName("playerId")] string PlayerId,
    [property: JsonPropertyName("userId")] string SteamId,
    [property: JsonPropertyName("ip")] string IpAddress,
    [property: JsonPropertyName("ping")] double Ping,
    [property: JsonPropertyName("location_x")] double LocationX,
    [property: JsonPropertyName("location_y")] double LocationY,
    [property: JsonPropertyName("level")] int Level,
    [property: JsonPropertyName("building_count")] int? BuildingCount);
