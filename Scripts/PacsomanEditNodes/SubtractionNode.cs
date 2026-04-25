namespace Example.Scripts.PacsomanEditNodes;

public partial class SubtractionNode(PacsomanEdit pacsomanEdit, float lhs = 0, float rhs = 0)
    : BinaryOperator<float, float, float>(pacsomanEdit, "Subtract", lhs, rhs)
{
    protected override float Operator(float lhs, float rhs)
    {
        return lhs - rhs;
    }
}
