namespace Example.Scripts.PacsomanEditNodes;

using Godot;

public partial class AbsoluteNode(PacsomanEdit pacsomanEdit, float value = 0)
    : UnaryOperator<float, float>(pacsomanEdit, "Absolute", value)
{
    protected override float Operator(float value)
    {
        return Mathf.Abs(value);
    }
}
