using ETor.App.Data;

namespace ETor.App.Trackers;

public delegate void AnnouncedEventHandler(TrackerData tracker, AnnouncedEventArgs e);
public delegate void AnnouncingEventHandler(TrackerData tracker);
public delegate void AnnounceFailedEventHandler(TrackerData tracker);