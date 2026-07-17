using System.Text.Json.Serialization;

namespace BlueDragonFly.PalWorldApiSDK.Core;

/// <summary>Lista de jugadores obtenida desde el servidor.</summary>
public sealed record PlayerList([property: JsonPropertyName("players")] IReadOnlyList<PlayerInfo> Players);
