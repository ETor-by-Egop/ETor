namespace ETor.App.Data;

[Flags]
public enum FileStatus
{
    None,
    Created,
    Downloading,
    Skip,
    Downloaded
}