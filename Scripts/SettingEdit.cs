using Godot;
using Pacsoman;

namespace Example.Scripts;

public partial class SettingEdit<TIDType, T>(
    PacsomanEditNode parent,
    Setting<TIDType, T> setting,
    int depth = 0
) : IndentedContent<VBoxContainer>(depth)
    where TIDType : notnull
    where T : IEquatable<T>
{
    public readonly PacsomanEditNode Parent = parent;

    public readonly Setting<TIDType, T> Setting = setting;

    private readonly Label IDLabel = new();

    private ValueEdit<T> ValueEdit = parent
        .PacsomanEdit.GetTypeInfo<T>()
        .TEditCreator(setting.DefaultValue);

    public override void _EnterTree()
    {
        base._EnterTree();

        Content.AddChild(IDLabel);
        IDLabel.SelfModulate = Colors.Gray;
        IDLabel.Text = $"{Setting.ID}:";

        ValueEdit.InternalValueChanged += OnInternalValueChanged;
        Content.AddChild(ValueEdit);
        Setting.SetValueProvider(ValueEdit);
        Setting.ActiveValueChanged += OnActiveValueChanged;
    }

    private void OnActiveValueChanged(Setting<TIDType, T> setting, T t1, T t2) { }

    private void OnInternalValueChanged()
    {
        Setting.DefaultValue = ValueEdit.InternalValue;
    }

    public override void _Ready()
    {
        base._Ready();
        Parent.CreateSlotLeft<T>(GetIndex(), ValueEdit);
    }
}
