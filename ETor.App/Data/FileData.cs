namespace ETor.App.Data;

public class FileData : IHashCoded
{
    public Guid InternalId { get; } = Guid.NewGuid();
    
    public string Path { get; }

    public long LengthBytes { get; }

    public long HashCode { get; private set; }
    public FileStatus Status { get; private set; }


    public FileData(string path, long lengthBytes)
    {
        Path = path;
        LengthBytes = lengthBytes;
    }

    public void SetStatus(FileStatus status)
    {
        Status = status;
        HashCode++;
    }
}