using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlueDragonFly.PalWorldApiSDK.Core;

internal sealed class GameActorJsonConverter : JsonConverter<GameActor>
{
    public override GameActor Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;
        var type = root.TryGetProperty("Type", out var typeProperty) ? typeProperty.GetString() : null;

        return type switch
        {
            "Character" => root.Deserialize<CharacterActor>(options) ?? throw new JsonException("El actor Character no es valido."),
            "PalBox" => root.Deserialize<PalBoxActor>(options) ?? throw new JsonException("El actor PalBox no es valido."),
            _ => new UnknownGameActor(type ?? string.Empty, root.Clone())
        };
    }

    public override void Write(Utf8JsonWriter writer, GameActor value, JsonSerializerOptions options) =>
        throw new NotSupportedException("La serializacion de actores del mundo no esta soportada.");
}
