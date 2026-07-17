using System.Text.Json.Serialization;

namespace BlueDragonFly.PalWorldApiSDK.Core;

/// <summary>Actor de tipo personaje, jugador, Pal o NPC.</summary>
public sealed record CharacterActor(
    string Type,
    string? InstanceID,
    string? UnitType,
    string? NickName,
    string? TrainerInstanceID,
    string? TrainerNickName,
    string? TrainerClass,
    [property: JsonPropertyName("userid")] string? SteamId,
    [property: JsonPropertyName("ip")] string? IpAddress,
    [property: JsonPropertyName("level")] int? Level,
    [property: JsonPropertyName("HP")] int? HealthPoints,
    [property: JsonPropertyName("MaxHP")] int? MaximumHealthPoints,
    string? GuildID,
    string? GuildName,
    string? Class,
    string? Action,
    [property: JsonPropertyName("AI_Action")] string? AiAction,
    float LocationX,
    float LocationY,
    float LocationZ,
    float? RotationX,
    float? RotationY,
    float? RotationZ,
    string? Stage,
    string? IsActive) : GameActor(Type);
