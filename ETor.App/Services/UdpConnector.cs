using System.Net;
using System.Net.Sockets;
using ETor.Configuration;
using ETor.Networking;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ETor.App.Services;

public interface IUdpSender
{
    Task<T?> SendReceive<T>(string host, int port, ICanSerialize request)
        where T : class, ICanDeserialize, new();
}

public class UdpSender : IUdpSender
{
    private readonly IOptions<NetworkConfig> _networkConfig;
    private readonly ILogger<UdpSender> _logger;

    public UdpSender(ILogger<UdpSender> logger, IOptions<NetworkConfig> networkConfig)
    {
        _logger = logger;
        _networkConfig = networkConfig;
    }

    public async Task<T?> SendReceive<T>(string host, int port, ICanSerialize request)
        where T : class, ICanDeserialize, new()
    {
        try
        {
            try
            {
                using var udp = new UdpClient();

                byte[] buffer = new byte[request.SerializedSize];
                request.Serialize(buffer);

                await udp.SendAsync(
                    buffer,
                    buffer.Length,
                    host,
                    port
                );

                using var source = new CancellationTokenSource(5000);

                var result = await udp.ReceiveAsync(source.Token);

                var response = new T();
                
                response.Deserialize(result.Buffer);

                _logger.LogInformation("Received Response: {@response}", response);

                return response;
            }
            catch (SocketException ex)
            {
                _logger.LogWarning(ex, "could not send message to UDP tracker {host}", host);
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

        return null;
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