namespace ETor.App.Data;

public class FileData
{
    public string Path { get; set; }

    public long LengthBytes { get; set; }

    public FileData(string path, long lengthBytes)
    {
        Path = path;
        LengthBytes = lengthBytes;
    }
}