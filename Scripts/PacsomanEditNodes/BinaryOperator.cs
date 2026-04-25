namespace Example.Scripts.PacsomanEditNodes;

public abstract partial class BinaryOperator<TIn1, TIn2, TOut>(
    PacsomanEdit pacsomanEdit,
    string title,
    TIn1 initialIn1,
    TIn2 initialIn2
) : Operator<TOut>(pacsomanEdit, title)
    where TIn1 : IEquatable<TIn1>
    where TIn2 : IEquatable<TIn2>
    where TOut : IEquatable<TOut>
{
    protected ValueEdit<TIn1>? In1 { get; private set; }
    protected ValueEdit<TIn2>? In2 { get; private set; }

    protected abstract TOut Operator(TIn1 in1, TIn2 in2);

    public override TOut GetValue()
    {
        return Operator(In1!.GetValue(), In2!.GetValue());
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        In1 = AddInput<TIn1>(0, initialIn1);
        In2 = AddInput<TIn2>(1, initialIn2);
    }
}
