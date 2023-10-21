using ETor.BEncoding;

namespace ETor.Manifest;

public class MultiFileSection : FileSection
{
    public MultiFileSection(BEncodeDictionary fileDictionary)
    {
        if (fileDictionary.TryGetValue<BEncodeInteger>("length", out var length))
        {
            Length = length.Value;
        }

        if (fileDictionary.TryGetValue<BEncodeList>("path", out var path))
        {
            Path = new List<string>(path.Items.Count);

            foreach (var pathItem in path.Items)
            {
                if (pathItem is not BEncodeString str)
                {
                    throw new InvalidOperationException("path is not a BEncoded string");
                }

                Path.Add(str.Value.ToString());
            }
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
            dict.Items["path"] = new BEncodeList(Path.Select(x => new BEncodeString(x)));
        }

        return dict;
    }
}