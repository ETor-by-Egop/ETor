using System.Net;
using System.Net.Sockets;
using ETor.Configuration;
using ETor.Networking;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ETor.App.Services;

public interface IUdpConnector
{
    Task ConnectTo(string host, int port);
}

public class UdpConnector : IUdpConnector
{
    private readonly IOptions<NetworkConfig> _networkConfig;
    private readonly ILogger<UdpConnector> _logger;

    public UdpConnector(ILogger<UdpConnector> logger, IOptions<NetworkConfig> networkConfig)
    {
        _logger = logger;
        _networkConfig = networkConfig;
    }

    public async Task ConnectTo(string host, int port)
    {
        try
        {
            // var address = DnsLookup(host);
            IPEndPoint? any = new IPEndPoint(IPAddress.Any, _networkConfig.Value.Port);

            try
            {
                using (UdpClient udp = new UdpClient())
                {
                    udp.Client.SendTimeout = (int) TimeSpan.FromSeconds(5)
                        .TotalMilliseconds;
                    udp.Client.ReceiveTimeout = (int) TimeSpan.FromSeconds(15)
                        .TotalMilliseconds;

                    var request = new UdpConnectRequest(Random.Shared.Next());

                    byte[] buffer = new byte[16];
                    request.Serialize(buffer);

                    await udp.SendAsync(
                        buffer,
                        buffer.Length,
                        host,
                        port
                    );

                    using var source = new CancellationTokenSource(5000);

                    var result = await udp.ReceiveAsync(source.Token);

                    var response = UdpConnectResponse.Deserialize(result.Buffer);

                    _logger.LogInformation("Received Response: {@response}", response);
                }
            }
            catch (SocketException ex)
            {
                _logger.LogWarning(ex, "could not send message to UDP tracker {host} for torrent", host);
            }
        }
        catch (Exception e)
        {
            _logger.LogWarning(
                e,
                "Failed to connect to {Host}:{Port}.",
                host,
                port
            );
        }
    }

    protected static IPAddress DnsLookup(string hostNameOrAddress)
    {
        IPHostEntry hostEntry = Dns.GetHostEntry(hostNameOrAddress);

        IPAddress[] ips = hostEntry.AddressList;

        if (ips.Length == 0)
        {
            throw new Exception($"Resolved {ips.Length} ips, which is not supported for host {hostNameOrAddress}");
        }

        return ips[0]
            .MapToIPv4();
    }
}