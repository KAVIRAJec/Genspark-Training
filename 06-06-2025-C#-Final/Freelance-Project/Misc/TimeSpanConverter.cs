using System.Text.Json;
using System.Text.Json.Serialization;

namespace Freelance_Project.Misc;
public class TimeSpanConverter : JsonConverter<TimeSpan?>
{
    public override TimeSpan? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var s = reader.GetString();
            if (TimeSpan.TryParse(s, out var ts))
                return ts;
        }
        return null;
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value?.ToString());
    }
}