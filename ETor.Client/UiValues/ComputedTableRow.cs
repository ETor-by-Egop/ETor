using ETor.App.Data;

namespace ETor.Client.UiValues;

public class ComputedTableRow<T>
    where T : class, IHashCoded
{
    protected T? Value;

    private long _computedWithHashCode = -1;

    protected bool IsDirty;

    public void UpdateIfNeeded(T value)
    {
        var newHashCode = value.HashCode;

        if (newHashCode != _computedWithHashCode)
        {
            _computedWithHashCode = newHashCode;
            Value = value;
            IsDirty = true;
        }
    }
}