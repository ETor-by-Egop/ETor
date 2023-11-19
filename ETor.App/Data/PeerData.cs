using System.Buffers.Binary;

namespace ETor.App.Data;

public class PeerData : IHashCoded
{
    public int Ip { get; set; }

    public short Port { get; set; }

    public long Address { get; set; }

    public long HashCode { get; private set; }

    public PeerData(Memory<byte> ipPort)
    {
        var ip = BinaryPrimitives.ReadInt32BigEndian(
            ipPort.Span
        );
        var port = BinaryPrimitives.ReadInt16BigEndian(
            ipPort.Slice(4, 2)
                .Span
        );

        Ip = ip;
        Port = port;

        Address = (long)ip << 16 | (ushort)port;
    }
}