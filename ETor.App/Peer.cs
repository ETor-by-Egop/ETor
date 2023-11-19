using ETor.App.Data;

namespace ETor.App;

public class Peer
{
    private readonly PeerData _peer;

    public Peer(PeerData peer)
    {
        _peer = peer;
    }
}