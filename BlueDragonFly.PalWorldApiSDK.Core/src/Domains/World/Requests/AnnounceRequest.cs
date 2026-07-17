using System.Text.Json.Serialization;

namespace BlueDragonFly.PalWorldApiSDK.Core;

internal sealed record AnnounceRequest([property: JsonPropertyName("message")] string Message);
