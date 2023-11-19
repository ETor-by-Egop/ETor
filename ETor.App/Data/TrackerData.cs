using System.Diagnostics;

namespace ETor.App.Data;

public class TrackerData : IHashCoded
{
    public Guid InternalId { get; set; } = Guid.NewGuid();

    public Uri Uri { get; set; }
    
    public string Url { get; private set;}

    public string Host { get; private set;}
    public int Port { get; private set;}

    public TrackerProtocol Protocol { get; private set; }

    public TrackerStatus Status { get; private set; }

    public long ConnectionId { get; private set; }
    public long HashCode { get; private set; }

    /// <summary>
    /// Announce interval in seconds
    /// </summary>
    public long UpdateInterval { get; private set; }


    public TrackerData(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            throw new InvalidOperationException($"Failed to parse tracker url: {url}");
        }

        Uri = uri;

        Url = url;
        Host = uri.Host;
        Port = uri.Port;

        Protocol = uri.Scheme.ToLower() switch
        {
            "udp" => TrackerProtocol.Udp,
            "tcp" => TrackerProtocol.Tcp,
            "wss" => TrackerProtocol.Wss,
            "http" => TrackerProtocol.Http,
            "https" => TrackerProtocol.Https,
            _ => throw new ArgumentOutOfRangeException(nameof(uri.Scheme), "Unknown tracker protocol")
        };
    }

    public void SetFailed()
    {
        Status = TrackerStatus.Failed;
        HashCode++;
    }

    public void SetUnsupported()
    {
        Status = TrackerStatus.Unsupported;
        HashCode++;
    }

    public void SetConnecting()
    {
        Status = TrackerStatus.Connecting;
        HashCode++;
    }

    public void SetRetrying()
    {
        Status = TrackerStatus.Retrying;
        HashCode++;
    }

    public void SetConnected(long connectionId)
    {
        ConnectionId = connectionId;
        Status = TrackerStatus.Connected;
        HashCode++;
    }

    public void SetAnnouncing()
    {
        Status = TrackerStatus.Announcing;
        HashCode++;
    }

    public void SetUpdateInterval(long interval)
    {
        UpdateInterval = interval;
        HashCode++;
    }

    public void SetAnnounced()
    {
        Status = TrackerStatus.Announced;
        HashCode++;
    }
}