namespace ETor.Client.UiValues;

public class ComputedTableRow<T>
{
    protected T? Value;

    protected bool IsDirty;

    public void UpdateIfNeeded(T? value)
    {
        if (Equals(Value, value)) return;

        Value = value;
        IsDirty = true;
    }
}