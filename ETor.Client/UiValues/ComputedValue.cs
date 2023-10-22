namespace ETor.Client.UiValues;

public abstract class ComputedValue<T>
{
    protected T? Value;
    
    protected bool IsDirty = true;
    protected string Computed { get; set; } = "";

    public void Calculate()
    {
        Computed = CalculateInternal();
        IsDirty = false;
    }

    protected abstract string CalculateInternal();

    public static implicit operator string(ComputedValue<T> val)
    {
        if (val.IsDirty)
        {
            val.Calculate();
        }

        return val.Computed;
    }

    public void UpdateIfNeeded(T? value)
    {
        if (Equals(Value, value)) return;

        Value = value;
        IsDirty = true;
    }
}

public interface IAutoComputedValueOf<in TObject>
{
    public void Fetch(TObject? value);

    public string Get();
}

public class AutoComputedValue<TObject>
{
    public static AutoComputedValue<TObject, T> Of<T>(Func<TObject, T> accessor, Func<T, string> converter)
    {
        return new AutoComputedValue<TObject, T>(accessor, converter);
    }
}

public class AutoComputedValue<TObject, T> : ComputedValue<T>, IAutoComputedValueOf<TObject>
{
    private Func<TObject?, T?> _accessor;
    private readonly Func<T?, string> _converter;

    public AutoComputedValue(Func<TObject?, T?> accessor, Func<T?, string> converter)
    {
        _accessor = accessor;
        _converter = converter;
    }

    protected override string CalculateInternal()
    {
        return _converter(Value);
    }

    public void Fetch(TObject? value)
    {
        UpdateIfNeeded(_accessor(value));
    }

    public string Get()
    {
        return this;
    }
}

public class IndexColumnOf<TObject> : AutoComputedValue<TObject, int>
{
    public IndexColumnOf(int index) : base(_ => index, x => x.ToString())
    {
    }
}