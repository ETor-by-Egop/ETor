using Open.Nat;

namespace ETor;

public class Networking
{
    public static async Task CreateUpnpMap()
    {
        var natDiscoverer = new NatDiscoverer();

        var device = await natDiscoverer.DiscoverDeviceAsync();

        await device.DeletePortMapAsync(
            new Mapping(
                Protocol.Tcp,
                50505,
                50505,
                0,
                "ETor"
            )
        );

        await device.CreatePortMapAsync(
            new Mapping(
                Protocol.Tcp,
                50505,
                50505,
                0,
                "ETor"
            )
        );
    }
}