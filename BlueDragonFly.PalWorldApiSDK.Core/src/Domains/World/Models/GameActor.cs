using System.Text.Json.Serialization;

namespace BlueDragonFly.PalWorldApiSDK.Core;

/// <summary>Actor presente en la instantanea del mundo.</summary>
[JsonConverter(typeof(GameActorJsonConverter))]
public abstract record GameActor(string Type);
