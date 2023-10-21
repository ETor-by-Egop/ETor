using ETor.BEncoding;
using Newtonsoft.Json;

namespace ETor.App;

public class BEncodeJsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is BEncodeDictionary dict)
        {
            serializer.Serialize(writer, dict.Items);
        }
        else if (value is BEncodeString str)
        {
            serializer.Serialize(writer, str.Value.ToString());
        }
        else if (value is BEncodeList list)
        {
            serializer.Serialize(writer, list.Items);
        }
        else if (value is BEncodeInteger i)
        {
            serializer.Serialize(writer, i.Value);
        }
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override bool CanConvert(Type objectType)
    {
        if (objectType.IsAssignableTo(typeof(BEncodeNode)))
        {
            return true;
        }

        return false;
    }
}