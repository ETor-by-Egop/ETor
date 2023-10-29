namespace ETor.Networking;

public enum UdpAction
{
    Connect = 0,
    Announce = 1,
    Scrape = 2,
    Error = 3 // (only in server replies)
}