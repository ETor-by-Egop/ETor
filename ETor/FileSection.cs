using ETor.BEncoding;

namespace ETor;

public abstract class FileSection
{
    public long? Length { get; set; }

    public List<string>? Path { get; set; }

    public abstract BEncodeNode BEncode();

    public string ComputeFilePath()
    {
        if (Path is null)
        {
            throw new InvalidOperationException("Can't compute file path, because torrent has no \"path\" defined");
        }

        return string.Join("\\", Path);
    }
}