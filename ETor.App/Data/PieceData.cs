namespace ETor.App.Data;

public class PieceData
{
    public Memory<byte> Hash { get; set; }

    public PieceStatus Status { get; set; }

    public PieceData(Memory<byte> hash)
    {
        Hash = hash;
    }
}