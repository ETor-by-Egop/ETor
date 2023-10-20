using ETor.BEncoding;

namespace ETor;

public class InfoSection
{
    public FilesSection? Files { get; set; }
    
    public string? Name { get; set; }

    public long? Length { get; set; }

    public long? PieceLength { get; set; }
    
    public byte[]? Pieces { get; set; }

    public bool IsSingleFile => Files is null;

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

        if (infoDictionary.TryGetValue<BEncodeInteger>("length", out var length))
        {
            Length = length.Value;
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

    public BEncodeNode BEncode()
    {
        var dict = new BEncodeDictionary();

        if (Files is not null)
        {
            dict.Items["files"] = Files.BEncode();
        }

        if (Name is not null)
        {
            dict.Items["name"] = new BEncodeString(Name);
        }

        if (PieceLength is not null)
        {
            dict.Items["piece length"] = new BEncodeInteger(PieceLength.Value);
        }

        if (Pieces is not null)
        {
            dict.Items["pieces"] = new BEncodeString(Pieces);
        }
        
        return dict;
    }

    public byte[] ComputeSha1()
    {
        var info = this.BEncode();

        var size = info.CalculateSize();
        
        // WARN: We assume here, that int32 max file size is equivalent to 2 Gb, which is probably never a case for a .torrent file

        using var ms = new MemoryStream(size);
        
        info.Serialize(ms);

        var bytes = ms.ToArray();

        return bytes.Sha1();
    }
}