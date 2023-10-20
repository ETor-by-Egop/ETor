using ETor.BEncoding;

namespace ETor;

public abstract class FileSection
{
    public long? Length { get; set; }

    public List<string>? Path { get; set; }

    public abstract BEncodeNode BEncode();
}