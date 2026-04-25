namespace Example.Scripts.PacsomanEditNodes;

public partial class BranchNode(PacsomanEdit pacsomanEdit)
    : PacsomanEditNode(pacsomanEdit, "Branch")
{
    protected ValueEdit<bool>? Condition { get; private set; }
    protected ValueEdit<float>? TrueValue { get; private set; }
    protected ValueEdit<float>? FalseValue { get; private set; }

    public event Action ValueChanged = delegate { };

    public float GetValue()
    {
        return Condition!.GetValue() ? TrueValue!.GetValue() : FalseValue!.GetValue();
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        Condition = AddInput<bool>(0, true);
        TrueValue = AddInput<float>(1, 0);
        FalseValue = AddInput<float>(2, 0);

        SetTypedTitlebarColor<float>();

        CreateSlotRight<float>(1, TrueValue);
        CreateSlotRight<float>(2, FalseValue);
        SetSlotEnabledRight(2, false);

        Condition.ValueChanged += OnConditionValueChanged;
    }

    private void OnConditionValueChanged()
    {
        if (Condition!.GetValue())
        {
            SetSlotEnabledRight(1, true);
            SetSlotEnabledRight(2, false);
            PacsomanEdit.ChangeProvider(this, 0, TrueValue!);
        }
        else
        {
            SetSlotEnabledRight(1, false);
            SetSlotEnabledRight(2, true);
            PacsomanEdit.ChangeProvider(this, 0, FalseValue!);
        }
    }

    protected override void OnInputChanged()
    {
        ValueChanged();
    }
}
