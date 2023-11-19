namespace ETor.App.Trackers;

public record AnnouncedEventArgs(int Interval, int Leechers, int Seeders, Memory<byte> IpPortPairs);