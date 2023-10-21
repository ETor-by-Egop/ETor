using System.Buffers.Binary;

namespace ETor.Networking;

public class UdpConnectResponse
{
    public int Action { get; }

    public int TransactionId { get; }

    public long ConnectionId { get; }

    public UdpConnectResponse(int action, int transactionId, long connectionId)
    {
        Action = action;
        TransactionId = transactionId;
        ConnectionId = connectionId;
    }

    public static UdpConnectResponse Deserialize(Span<byte> buffer)
    {
        if (buffer.Length < 16)
        {
            throw new InvalidOperationException($"Received UdpConnectResponse less than 16 bytes. Was {buffer.Length}");
        }

        var action = BinaryPrimitives.ReadInt32BigEndian(buffer);
        var transactionId = BinaryPrimitives.ReadInt32BigEndian(buffer[4..]);
        var connectionId = BinaryPrimitives.ReadInt64BigEndian(buffer[8..]);

        return new UdpConnectResponse(action, transactionId, connectionId);
    }
}