using System.Text.Json;

namespace BlueDragonFly.PalWorldApiSDK.Core;

/// <summary>Actor de un tipo que el SDK aun no conoce.</summary>
public sealed record UnknownGameActor(string Type, JsonElement RawData) : GameActor(Type);
