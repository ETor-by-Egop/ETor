using ETor;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Open.Nat;

var natDiscoverer = new NatDiscoverer();

var device = await natDiscoverer.DiscoverDeviceAsync();

await device.DeletePortMapAsync(
    new Mapping(
        Protocol.Tcp,
        50505,
        50505,
        0,
        "ETor"
    )
);
await device.CreatePortMapAsync(
    new Mapping(
        Protocol.Tcp,
        50505,
        50505,
        0,
        "ETor"
    )
);

//
// var content = await File.ReadAllBytesAsync("C:\\Users\\Admin\\Downloads\\microsoft-windows-10_0_19045_2006-version-22h2-msdn-ru.torrent");
//
// var encodedContent = new BEncodedContent(content);
//
// var dict = encodedContent.ReadDictionary();
//
// var obj = JObject.FromObject(dict, JsonSerializer.Create()
//     .WithMyConverter()
//     .WithEnumAsString());
//
// var pieces = dict["info"]["pieces"] as BEncodeString;
//
// var piecesCount = pieces.Value.Value.Count / 20;
//
// Console.WriteLine($"Detected {piecesCount} pieces");
//
// for (var i = 0; i < piecesCount; i++)
// {
//     var piece = pieces.Value.Value.Slice(20 * i, 20);
//     Console.WriteLine($"P{i}: {string.Join("", piece.Select(x => x.ToString("X2")))}");
// }

// Console.WriteLine(obj.ToString(Formatting.Indented));

// Console.WriteLine(encodedContent.ReadString());

// Console.WriteLine(Encoding.UTF8.GetString(content));

public class MyConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is BEncodeDictionary dict)
        {
            serializer.Serialize(writer, dict.Items);
        }
        else if (value is BEncodeString str)
        {
            serializer.Serialize(writer, str.Value);
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

public static class Extensions
{
    public static JsonSerializer WithMyConverter(this JsonSerializer serializer)
    {
        serializer.Converters.Insert(0, new MyConverter());

        return serializer;
    }

    public static JsonSerializer WithEnumAsString(this JsonSerializer serializer)
    {
        serializer.Converters.Insert(0, new StringEnumConverter());

        return serializer;
    }
}