using System.Buffers.Binary;

namespace ETor.Networking;

public class UdpAnnounceResponse : ICanDeserialize
{
    /// <summary>
    /// The action this is a reply to.
    /// </summary>
    public UdpAction Action { get; set; }

    /// <summary>
    /// Must match the transaction_id sent in the announce request.
    /// </summary>
    public int TransactionId { get; set; }

    /// <summary>
    /// The number of seconds you should wait until re-announcing yourself.
    /// </summary>
    public int Interval { get; set; }

    /// <summary>
    /// The number of peers in the swarm that has not finished downloading.
    /// </summary>
    public int Leechers { get; set; }

    /// <summary>
    /// The number of peers in the swarm that has finished downloading and are seeding.
    /// </summary>
    public int Seeders { get; set; }

    /// <summary>
    /// Pairs of
    /// <para>int32: The ip of a peer in the swarm.</para>
    /// <para>int16: The peer's listen port.</para>
    /// </summary>
    public Memory<byte> IpPortPairs { get; set; }

    public void Deserialize(Memory<byte> buffer)
    {
        Action = (UdpAction)BinaryPrimitives.ReadInt32BigEndian(buffer.Span);
        TransactionId = BinaryPrimitives.ReadInt32BigEndian(buffer[4..].Span);
        Interval = BinaryPrimitives.ReadInt32BigEndian(buffer[8..].Span);
        Leechers = BinaryPrimitives.ReadInt32BigEndian(buffer[12..].Span);
        Seeders = BinaryPrimitives.ReadInt32BigEndian(buffer[16..].Span);
        IpPortPairs = buffer[20..];
    }
}