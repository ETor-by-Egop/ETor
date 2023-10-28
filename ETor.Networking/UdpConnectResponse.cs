using System.Buffers.Binary;

namespace ETor.Networking;

public class UdpConnectResponse : ICanDeserialize
{
    public int Action { get; private set; }

    public int TransactionId { get; private set; }

    public long ConnectionId { get; private set; }

    public UdpConnectResponse()
    {
    }

    public UdpConnectResponse(int action, int transactionId, long connectionId)
    {
        Action = action;
        TransactionId = transactionId;
        ConnectionId = connectionId;
    }

    public void Deserialize(Span<byte> buffer)
    {
        if (buffer.Length < 16)
        {
            throw new InvalidOperationException($"Received UdpConnectResponse less than 16 bytes. Was {buffer.Length}");
        }

        Action = BinaryPrimitives.ReadInt32BigEndian(buffer);
        TransactionId = BinaryPrimitives.ReadInt32BigEndian(buffer[4..]);
        ConnectionId = BinaryPrimitives.ReadInt64BigEndian(buffer[8..]);
    }
}

public interface ICanDeserialize
{
    void Deserialize(Span<byte> buffer);
}