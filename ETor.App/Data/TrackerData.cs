using System.Diagnostics;

namespace ETor.App.Data;

public class TrackerData
{
    public string Url { get; set; }

    public TrackerProtocol Protocol { get; set; }

    public TrackerStatus Status { get; set; }

    public long LastConnectedAt { get; set; } = Stopwatch.GetTimestamp();

    public long ConnectionId { get; set; }
    
    public TrackerData(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            throw new InvalidOperationException($"Failed to parse tracker url: {url}");
        }

        Url = url;

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
}