using System.Diagnostics;

namespace ETor.App.Data;

public class TrackerData : IHashCoded
{
    public string Url { get; private set;}

    public string Host { get; private set;}
    public int Port { get; private set;}

    public TrackerProtocol Protocol { get; private set; }

    public TrackerStatus Status { get; private set; }

    public long LastConnectedAt { get; private set; } = -1;

    public long ConnectionId { get; private set; }
    public long HashCode { get; private set; }


    public TrackerData(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            throw new InvalidOperationException($"Failed to parse tracker url: {url}");
        }

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

    public void SetStatus(TrackerStatus status)
    {
        Status = status;
        HashCode++;
    }

    public void SetConnected(long connectionId, long timestamp)
    {
        ConnectionId = connectionId;
        Status = TrackerStatus.Connected;
        LastConnectedAt = timestamp;
        HashCode++;
    }
}