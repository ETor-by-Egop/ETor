using ETor.BEncoding;

namespace ETor;

public class InfoSection
{
    public FilesSection? Files { get; set; }
    
    public string? Name { get; set; }

    public long? PieceLength { get; set; }
    
    public byte[]? Pieces { get; set; }

    public InfoSection(BEncodeDictionary infoDictionary)
    {
        if (infoDictionary.TryGetValue<BEncodeList>("files", out var files))
        {
            Files = new FilesSection(files);
        }

        if (infoDictionary.TryGetValue<BEncodeString>("name", out var name))
        {
            Name = name.Value.ToString();
        }

        if (infoDictionary.TryGetValue<BEncodeInteger>("piece length", out var pieceLength))
        {
            PieceLength = pieceLength.Value;
        }

        if (infoDictionary.TryGetValue<BEncodeString>("pieces", out var pieces))
        {
            Pieces = pieces.Value.Value.ToArray();
        }
    }
}