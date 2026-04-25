using Godot;
using Pacsoman;

namespace Example.Scripts.ValueEdits;

public partial class BoolEdit(bool initialValue) : ValueEdit<bool>(initialValue)
{
    private CheckButton ValueEdit = new();

    public override void _EnterTree()
    {
        base._EnterTree();
        AddChild(ValueEdit);
        ValueEdit.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        ValueEdit.Text = GetValue().ToString();
        ValueEdit.ButtonPressed = GetValue();
        ValueEdit.Toggled += OnToggled;
        ValueChanged += OnValueChanged;
        ValueProviderChanged += OnValueProviderChanged;
    }

    private void OnValueChanged()
    {
        ValueEdit.Text = GetValue().ToString();
        ValueEdit.ButtonPressed = GetValue();
    }

    private void OnValueProviderChanged(IValueProvider? provider)
    {
        ValueEdit.Disabled = provider != null;
    }

    private void OnToggled(bool toggledOn)
    {
        InternalValue = toggledOn;
    }
}
