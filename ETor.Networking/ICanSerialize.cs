namespace ETor.Networking;

public interface ICanSerialize
{
    void Serialize(Span<byte> buffer);
    
    int SerializedSize { get; }
}