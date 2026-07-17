using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlueDragonFly.PalWorldApiSDK.Core;

#pragma warning disable CS1591

/// <summary>Configuracion expuesta por <c>GET /v1/api/settings</c>.</summary>
/// <remarks>Las propiedades son opcionales para tolerar diferencias entre versiones del servidor.</remarks>
public sealed class ServerSettings
{
    public string? Difficulty { get; init; }
    public double? DayTimeSpeedRate { get; init; }
    public double? NightTimeSpeedRate { get; init; }
    public double? ExpRate { get; init; }
    public double? PalCaptureRate { get; init; }
    public double? PalSpawnNumRate { get; init; }
    public double? PalDamageRateAttack { get; init; }
    public double? PalDamageRateDefense { get; init; }
    public double? PlayerDamageRateAttack { get; init; }
    public double? PlayerDamageRateDefense { get; init; }
    public double? PlayerStomachDecreaceRate { get; init; }
    public double? PlayerStaminaDecreaceRate { get; init; }
    public double? PlayerAutoHPRegeneRate { get; init; }
    public double? PlayerAutoHpRegeneRateInSleep { get; init; }
    public double? PalStomachDecreaceRate { get; init; }
    public double? PalStaminaDecreaceRate { get; init; }
    public double? PalAutoHPRegeneRate { get; init; }
    public double? PalAutoHpRegeneRateInSleep { get; init; }
    public double? BuildObjectDamageRate { get; init; }
    public double? BuildObjectDeteriorationDamageRate { get; init; }
    public double? CollectionDropRate { get; init; }
    public double? CollectionObjectHpRate { get; init; }
    public double? CollectionObjectRespawnSpeedRate { get; init; }
    public double? EnemyDropItemRate { get; init; }
    public string? DeathPenalty { get; init; }
    public bool? BEnablePlayerToPlayerDamage { get; init; }
    public bool? BEnableFriendlyFire { get; init; }
    public bool? BEnableInvaderEnemy { get; init; }
    public bool? BActiveUNKO { get; init; }
    public bool? BEnableAimAssistPad { get; init; }
    public bool? BEnableAimAssistKeyboard { get; init; }
    public double? DropItemMaxNum { get; init; }
    public double? DropItemMaxNum_UNKO { get; init; }
    public double? BaseCampMaxNum { get; init; }
    public double? BaseCampWorkerMaxNum { get; init; }
    public double? DropItemAliveMaxHours { get; init; }
    public bool? BAutoResetGuildNoOnlinePlayers { get; init; }
    public double? AutoResetGuildTimeNoOnlinePlayers { get; init; }
    public double? GuildPlayerMaxNum { get; init; }
    public double? PalEggDefaultHatchingTime { get; init; }
    public double? WorkSpeedRate { get; init; }
    public bool? BIsMultiplay { get; init; }
    public bool? BIsPvP { get; init; }
    public bool? BCanPickupOtherGuildDeathPenaltyDrop { get; init; }
    public bool? BEnableNonLoginPenalty { get; init; }
    public bool? BEnableFastTravel { get; init; }
    public bool? BIsStartLocationSelectByMap { get; init; }
    public bool? BExistPlayerAfterLogout { get; init; }
    public bool? BEnableDefenseOtherGuildPlayer { get; init; }
    public double? CoopPlayerMaxNum { get; init; }
    public double? ServerPlayerMaxNum { get; init; }
    public string? ServerName { get; init; }
    public string? ServerDescription { get; init; }
    public double? PublicPort { get; init; }
    public string? PublicIP { get; init; }
    public bool? RCONEnabled { get; init; }
    public double? RCONPort { get; init; }
    public string? Region { get; init; }
    public bool? BUseAuth { get; init; }
    public string? BanListURL { get; init; }
    public bool? RESTAPIEnabled { get; init; }
    public double? RESTAPIPort { get; init; }
    public bool? BShowPlayerList { get; init; }
    public string? AllowConnectPlatform { get; init; }
    public bool? BIsUseBackupSaveData { get; init; }
    public string? LogFormatType { get; init; }

    /// <summary>Propiedades no reconocidas, preservadas para compatibilidad con nuevas versiones.</summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalSettings { get; init; }
}

#pragma warning restore CS1591
