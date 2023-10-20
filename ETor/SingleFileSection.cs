using ETor.BEncoding;

namespace ETor;

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
}