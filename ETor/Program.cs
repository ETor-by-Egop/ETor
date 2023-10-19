using ETor;
using ETor.BEncoding;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Open.Nat;

// var natDiscoverer = new NatDiscoverer();
//
// var device = await natDiscoverer.DiscoverDeviceAsync();
//
// await device.DeletePortMapAsync(
//     new Mapping(
//         Protocol.Tcp,
//         50505,
//         50505,
//         0,
//         "ETor"
//     )
// );
//
// await device.CreatePortMapAsync(
//     new Mapping(
//         Protocol.Tcp,
//         50505,
//         50505,
//         0,
//         "ETor"
//     )
// );

//
var content = await File.ReadAllBytesAsync("C:\\Users\\Admin\\Downloads\\microsoft-windows-10_0_19045_2006-version-22h2-msdn-ru.torrent");

var encodedContent = new BEncodedContent(content);

var dict = encodedContent.ReadDictionary();

var obj = JObject.FromObject(dict, JsonSerializer.Create()
    .WithMyConverter()
    .WithEnumAsString());

var pieces = dict["info"]["pieces"] as BEncodeString;

var piecesCount = pieces.Value.Value.Count / 20;

Console.WriteLine($"Detected {piecesCount} pieces");

for (var i = 0; i < piecesCount; i++)
{
    var piece = pieces.Value.Value.Slice(20 * i, 20);
    Console.WriteLine($"P{i}: {piece.ToHexString()}");
}

Console.WriteLine(obj.ToString(Formatting.Indented));

public static class Extensions
{
    public static JsonSerializer WithMyConverter(this JsonSerializer serializer)
    {
        serializer.Converters.Insert(0, new BEncodeJsonConverter());

        return serializer;
    }

    public static JsonSerializer WithEnumAsString(this JsonSerializer serializer)
    {
        serializer.Converters.Insert(0, new StringEnumConverter());

        return serializer;
    }

    public static string ToHexString(this ArraySegment<byte> bytes)
    {
        return string.Create(
            bytes.Count * 2,
            bytes,
            (span, segment) =>
            {
                for (var index = 0; index < segment.Count; index++)
                {
                    var c = segment[index];
                    if (!c.TryFormat(span[(index * 2)..], out var written, "X2") || written < 2)
                    {
                        throw new InvalidOperationException("Failed to format bytes as hex");
                    }
                }
            }
        );
    }
}