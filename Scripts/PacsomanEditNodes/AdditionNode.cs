namespace Example.Scripts.PacsomanEditNodes;

public partial class AdditionNode(PacsomanEdit pacsomanEdit, float lhs = 0, float rhs = 0)
    : BinaryOperator<float, float, float>(pacsomanEdit, "Add", lhs, rhs)
{
    protected override float Operator(float lhs, float rhs)
    {
        return lhs + rhs;
    }
}
