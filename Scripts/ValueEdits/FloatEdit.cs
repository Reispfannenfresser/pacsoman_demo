using Godot;
using Pacsoman;

namespace Example.Scripts.ValueEdits;

public partial class FloatEdit(float initialValue) : ValueEdit<float>(initialValue)
{
    private LineEdit ValueEdit = new();

    public override void _EnterTree()
    {
        base._EnterTree();
        AddChild(ValueEdit);
        ValueEdit.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        ValueEdit.Alignment = HorizontalAlignment.Right;
        ValueEdit.Text = GetValue().ToString();
        ValueEdit.TextSubmitted += OnTextSubmitted;
        ValueEdit.FocusExited += OnFocusExited;
        ValueChanged += OnValueChanged;
        ValueProviderChanged += OnValueProviderChanged;
    }

    private void OnValueChanged()
    {
        ValueEdit.Text = GetValue().ToString();
    }

    private void OnValueProviderChanged(IValueProvider? provider)
    {
        ValueEdit.Editable = provider == null;
    }

    private void OnFocusExited()
    {
        if (ValueEdit.Editable)
        {
            UpdateInternalValue();
        }
    }

    private void OnTextSubmitted(string _)
    {
        UpdateInternalValue();
    }

    private void UpdateInternalValue()
    {
        if (
            float.TryParse(ValueEdit.Text, out float value)
            && !float.IsInfinity(value)
            && !float.IsNaN(value)
        )
        {
            InternalValue = value;
        }
        else
        {
            ValueEdit.Text = GetValue().ToString();
        }
    }
}
