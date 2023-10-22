using System.Collections;

namespace ETor.Shared;

public class SelectableReadOnlyList<T> : IReadOnlyList<T>
{
    public int SelectedIndex { get; set; } = -1;

    private readonly IReadOnlyList<T> _list;

    public SelectableReadOnlyList(IReadOnlyList<T> list)
    {
        _list = list;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count => _list.Count;

    public T this[int index] => _list[index];
}