using System.Text.Json.Serialization;

namespace BlueDragonFly.PalWorldApiSDK.Core;

/// <summary>Informacion general de un servidor Palworld.</summary>
public sealed record ServerInfo(
    [property: JsonPropertyName("version")] string Version,
    [property: JsonPropertyName("servername")] string ServerName,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("worldguid")] string WorldGuid);
