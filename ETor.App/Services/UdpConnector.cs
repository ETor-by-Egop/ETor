using System.Net.Sockets;
using ETor.Networking;

namespace ETor.App.Services;

public static class UdpSender
{
    public static async Task<T?> SendReceive<T>(this UdpClient client, string host, int port, Memory<byte> request, CancellationToken cancellationToken)
        where T : class, ICanDeserialize, new()
    {
        try
        {
            await client.SendAsync(
                request,
                host,
                port,
                cancellationToken
            );

            var result = await client.ReceiveAsync(cancellationToken);

            var response = new T();

            response.Deserialize(result.Buffer);

            return response;
        }
        catch (Exception e)
        {
            return null;
        }
    }
}