namespace Example.Scripts.PacsomanEditNodes;

public partial class LesserNode(PacsomanEdit pacsomanEdit, float lhs = 0, float rhs = 0)
    : BinaryOperator<float, float, bool>(pacsomanEdit, "Lesser", lhs, rhs)
{
    protected override bool Operator(float lhs, float rhs)
    {
        return lhs < rhs;
    }
}
