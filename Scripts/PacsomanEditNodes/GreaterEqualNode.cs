namespace Example.Scripts.PacsomanEditNodes;

public partial class GreaterEqualNode(PacsomanEdit pacsomanEdit, float lhs = 0, float rhs = 0)
    : BinaryOperator<float, float, bool>(pacsomanEdit, "Greater or equal", lhs, rhs)
{
    protected override bool Operator(float lhs, float rhs)
    {
        return lhs >= rhs;
    }
}
