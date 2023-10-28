using ETor.App.Services;

namespace ETor.App.DelayedTasks;

public interface IDelayedTask
{
    Task ExecuteAsync(IDelayer delayer);
}