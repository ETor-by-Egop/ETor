using ETor.BEncoding;

namespace ETor.Manifest;

public class SingleFileSection : FileSection
{
    public SingleFileSection(BEncodeDictionary fileDictionary)
    {
        if (fileDictionary.TryGetValue<BEncodeInteger>("length", out var length))
        {
            Length = length.Value;
        }

        if (fileDictionary.TryGetValue<BEncodeString>("path", out var path))
        {
            Path = new List<string>(1);

            Path.Add(path.Value.ToString());
        }
    }
    
    public override BEncodeNode BEncode()
    {
        var dict = new BEncodeDictionary();

        if (Length is not null)
        {
            dict.Items["length"] = new BEncodeInteger(Length.Value);
        }

        if (Path is not null)
        {
            dict.Items["path"] = new BEncodeString(Path[0]);
        }

        return dict;
    }
}