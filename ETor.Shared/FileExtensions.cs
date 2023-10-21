namespace ETor.Shared;

public static class FileExtensions
{
    public static bool IsDirectory(this FileSystemInfo info)
    {
        // get the file attributes for file or directory
        FileAttributes attr = info.Attributes;

        //detect whether its a directory or file
        return (attr & FileAttributes.Directory) == FileAttributes.Directory;
    }
}