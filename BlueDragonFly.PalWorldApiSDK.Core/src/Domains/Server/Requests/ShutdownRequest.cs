using System.Text.Json.Serialization;

namespace BlueDragonFly.PalWorldApiSDK.Core;

internal sealed record ShutdownRequest(
    [property: JsonPropertyName("waittime")] int WaitTimeSeconds,
    [property: JsonPropertyName("message")] string? Message);
