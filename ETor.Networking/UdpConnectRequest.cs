using System.Buffers.Binary;
using System.Runtime.Serialization;

namespace ETor.Networking;

public record UdpConnectRequest(int TransactionId) : ICanSerialize
{
    /// <summary>
    /// Must be initialized to 0x41727101980 in network byte order. This will identify the protocol.
    /// </summary>

    public long ConnectionId => 0x41727101980;

    public UdpAction Action => UdpAction.Connect;

    public void Serialize(Span<byte> buffer)
    {
        BinaryPrimitives.WriteInt64BigEndian(buffer, ConnectionId);
        BinaryPrimitives.WriteInt32BigEndian(buffer[8..], (int)Action);
        BinaryPrimitives.WriteInt32BigEndian(buffer[12..], TransactionId);
    }

    public int SerializedSize { get; } = 16;
}