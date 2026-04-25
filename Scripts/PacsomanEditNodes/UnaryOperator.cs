namespace Example.Scripts.PacsomanEditNodes;

public abstract partial class UnaryOperator<TIn, TOut>(
    PacsomanEdit pacsomanEdit,
    string title,
    TIn initialIn
) : Operator<TOut>(pacsomanEdit, title)
    where TIn : IEquatable<TIn>
    where TOut : IEquatable<TOut>
{
    protected ValueEdit<TIn>? In1 { get; private set; }

    protected abstract TOut Operator(TIn value);

    public override TOut GetValue()
    {
        return Operator(In1!.GetValue());
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        In1 = AddInput<TIn>(0, initialIn);
    }
}
