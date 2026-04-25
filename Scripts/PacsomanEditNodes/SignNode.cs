namespace Example.Scripts.PacsomanEditNodes;

using Godot;

public partial class SignNode(PacsomanEdit pacsomanEdit, float value = 0)
    : UnaryOperator<float, float>(pacsomanEdit, "Sign", value)
{
    protected override float Operator(float value)
    {
        return Mathf.Sign(value);
    }
}
