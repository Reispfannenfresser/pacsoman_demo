using Godot;

namespace Example.Scripts.PacsomanEditNodes;

public partial class MinimumNode(PacsomanEdit pacsomanEdit, float lhs = 0, float rhs = 0)
    : BinaryOperator<float, float, float>(pacsomanEdit, "Minimum", lhs, rhs)
{
    protected override float Operator(float lhs, float rhs)
    {
        return Mathf.Min(lhs, rhs);
    }
}
