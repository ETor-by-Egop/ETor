namespace ETor.App.Data;

public class PieceData : IHashCoded
{
    public Memory<byte> Hash { get; }

    public PieceStatus Status { get; private set; }

    public long HashCode { get; private set; }

    public PieceData(Memory<byte> hash)
    {
        Hash = hash;
    }

    public void SetStatus(PieceStatus status)
    {
        Status = status;
        HashCode++;
    }
}