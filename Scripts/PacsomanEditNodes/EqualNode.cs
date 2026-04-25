namespace Example.Scripts.PacsomanEditNodes;

public partial class EqualNode(PacsomanEdit pacsomanEdit, float lhs = 0, float rhs = 0)
    : BinaryOperator<float, float, bool>(pacsomanEdit, "Equal", lhs, rhs)
{
    protected override bool Operator(float lhs, float rhs)
    {
        return lhs == rhs;
    }
}
