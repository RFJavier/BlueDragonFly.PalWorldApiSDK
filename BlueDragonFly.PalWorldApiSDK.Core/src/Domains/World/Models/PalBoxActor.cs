namespace BlueDragonFly.PalWorldApiSDK.Core;

/// <summary>Actor de tipo PalBox.</summary>
public sealed record PalBoxActor(
    string Type,
    string? GuildID,
    string? GuildName,
    string? Class,
    float? LocationX,
    float? LocationY,
    float? LocationZ) : GameActor(Type);
