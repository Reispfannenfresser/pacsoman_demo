using Godot;

namespace Example.Scripts.PacsomanEditNodes;

public partial class MaximumNode(PacsomanEdit pacsomanEdit, float lhs = 0, float rhs = 0)
    : BinaryOperator<float, float, float>(pacsomanEdit, "Maximum", lhs, rhs)
{
    protected override float Operator(float lhs, float rhs)
    {
        return Mathf.Max(lhs, rhs);
    }
}
