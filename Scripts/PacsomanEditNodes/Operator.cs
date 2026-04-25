using Pacsoman;

namespace Example.Scripts.PacsomanEditNodes;

public abstract partial class Operator<TOut>(PacsomanEdit pacsomanEdit, string title)
    : PacsomanEditNode(pacsomanEdit, title),
        IValueProvider<TOut>
    where TOut : IEquatable<TOut>
{
    public event Action ValueChanged = delegate { };

    public abstract TOut GetValue();

    public override void _EnterTree()
    {
        base._EnterTree();
        SetTypedTitlebarColor<TOut>();
        CreateSlotRight<TOut>(0, this);
    }

    protected override void OnInputChanged()
    {
        base.OnInputChanged();
        ValueChanged();
    }
}
