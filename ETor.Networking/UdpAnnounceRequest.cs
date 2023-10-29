using System.Buffers.Binary;

namespace ETor.Networking;

public class UdpAnnounceRequest : ICanSerialize
{
    /// <summary>
    /// The connection id acquired from establishing the connection.
    /// </summary>
    public long ConnectionId { get; set; }

    // Action.
    public UdpAction Action => UdpAction.Announce;

    /// <summary>
    /// Randomized by client.
    /// </summary>
    public int TransactionId { get; set; }

    /// <summary>
    /// The info-hash of the torrent you want announce yourself in.
    /// </summary>
    public Memory<byte> InfoHash { get; set; }

    /// <summary>
    /// Your peer id.
    /// </summary>
    public Memory<byte> PeerId { get; set; }

    /// <summary>
    /// The number of byte you've downloaded in this session.
    /// </summary>
    public long Downloaded { get; set; }

    /// <summary>
    /// The number of bytes you have left to download until you're finished.
    /// </summary>
    public long Left { get; set; }

    // The number of bytes you have uploaded in this session.
    public long Uploaded { get; set; }

    /// <summary>
    /// The event
    /// </summary>
    public UdpEvent Event { get; set; }

    /// <summary>
    /// Your ip address. Set to 0 if you want the tracker to use the sender of this UDP packet.
    /// </summary>
    public int IpAddress { get; set; }

    /// <summary>
    /// A unique key that is randomized by the client.
    /// </summary>
    public int Key { get; set; }

    /// <summary>
    /// The maximum number of peers you want in the reply. Use -1 for default.
    /// </summary>
    public int NumWant { get; set; }

    /// <summary>
    /// The port you're listening on.
    /// </summary>
    public short Port { get; set; }

    public void Serialize(Span<byte> buffer)
    {
        if (buffer.Length < SerializedSize)
        {
            throw new InvalidOperationException("Udp Announce Request can't be serialized into a buffer, because it's too small.");
        }
        
        BinaryPrimitives.WriteInt64BigEndian(buffer, ConnectionId);
        BinaryPrimitives.WriteInt32BigEndian(buffer[8..], (int)Action);
        BinaryPrimitives.WriteInt32BigEndian(buffer[12..], TransactionId);
        InfoHash.Span.CopyTo(buffer[16..]);
        PeerId.Span.CopyTo(buffer[36..]);
        BinaryPrimitives.WriteInt64BigEndian(buffer[56..], Downloaded);
        BinaryPrimitives.WriteInt64BigEndian(buffer[64..], Left);
        BinaryPrimitives.WriteInt64BigEndian(buffer[72..], Uploaded);
        BinaryPrimitives.WriteInt32BigEndian(buffer[80..], (int)Event);
        BinaryPrimitives.WriteInt32BigEndian(buffer[84..], IpAddress);
        BinaryPrimitives.WriteInt32BigEndian(buffer[88..], Key);
        BinaryPrimitives.WriteInt32BigEndian(buffer[92..], NumWant);
        BinaryPrimitives.WriteInt16BigEndian(buffer[96..], Port);
    }

    public int SerializedSize { get; } = 98;
}