using System.Buffers.Binary;
using System.Runtime.Serialization;

namespace ETor.Networking;

public record UdpConnectRequest(int TransactionId) : ICanSerialize
{
    public long ConnectionId => 0x41727101980; // magic constant, every request should have connection_id, but for the connect it's a magic, because it has not yet been obtained

    public int Action => 0; // connect;

    public void Serialize(Span<byte> buffer)
    {
        BinaryPrimitives.WriteInt64BigEndian(buffer, ConnectionId);
        BinaryPrimitives.WriteInt32BigEndian(buffer[8..], Action);
        BinaryPrimitives.WriteInt32BigEndian(buffer[12..], TransactionId);
    }

    public int SerializedSize { get; } = 16;
}