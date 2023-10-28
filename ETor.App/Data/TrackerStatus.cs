namespace ETor.App.Data;

public enum TrackerStatus
{
    Unknown,
    Unsupported,
    Connecting,
    Connected,
    Retrying,
    Failed,
    Announcing,
    Announced
}