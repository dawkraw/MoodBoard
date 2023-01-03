using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using ValueOf;

namespace Domain.ValueObjects;

public class CanvasVector : ValueOf<Vector2, CanvasVector>
{
    private readonly Vector2 _minimalCoordinates = new(-10000, -10000);

    protected override void Validate()
    {
        if (Value.X < _minimalCoordinates.X || Value.Y < _minimalCoordinates.Y)
            throw new ArgumentException("Coordinates can't be lower than -10000");
    }

    public static CanvasVector ConvertFromString(string input)
    {
        var coordinates = input.Substring(1, input.Length - 2).Split(",");
        var x = float.Parse(coordinates[0]);
        var y = float.Parse(coordinates[1]);
        return From(new Vector2(x, y));
    }
}

public class CanvasVectorJsonConverter : JsonConverter<CanvasVector>
{
    public override CanvasVector Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return CanvasVector.ConvertFromString(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, CanvasVector value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("x", value.Value.X);
        writer.WriteNumber("y", value.Value.Y);

        writer.WriteEndObject();
    }
}