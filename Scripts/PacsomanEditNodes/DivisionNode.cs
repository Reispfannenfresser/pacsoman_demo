namespace Example.Scripts.PacsomanEditNodes;

public partial class DivisionNode(PacsomanEdit pacsomanEdit, float lhs = 0, float rhs = 1)
    : BinaryOperator<float, float, float>(pacsomanEdit, "Divide", lhs, rhs)
{
    protected override void OnInputChanged()
    {
        float rhsValue = In2!.GetValue();
        if (rhsValue != 0)
        {
            base.OnInputChanged();
        }
        SetSlotEnabledRight(0, rhsValue != 0);
    }

    protected override float Operator(float lhs, float rhs)
    {
        return lhs / rhs;
    }
}
