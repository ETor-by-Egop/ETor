using System.Net;
using System.Net.Sockets;

namespace ETor.Networking;

public class Tracker
{
    public string Url { get; set; }
    
    public bool IsUdp { get; set; }
    public bool IsTcp { get; set; }
    public bool IsWss { get; set; }
    public bool IsHttp { get; set; }
    public bool IsHttps { get; set; }

    public string Host { get; set; }

    public int Port { get; set; }
    
    public Tracker(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            throw new InvalidOperationException($"Failed to parse tracker url: {url}");
        }

        Url = url;

        IsUdp = string.Compare(uri.Scheme, "udp", StringComparison.OrdinalIgnoreCase) == 0;
        IsTcp = string.Compare(uri.Scheme, "tcp", StringComparison.OrdinalIgnoreCase) == 0;
        IsWss = string.Compare(uri.Scheme, "wss", StringComparison.OrdinalIgnoreCase) == 0;
        IsHttp = string.Compare(uri.Scheme, "http", StringComparison.OrdinalIgnoreCase) == 0;
        IsHttps = string.Compare(uri.Scheme, "https", StringComparison.OrdinalIgnoreCase) == 0;
        Host = uri.Host;
        Port = uri.Port;
    }
}