using Godot;
using Pacsoman;

namespace Example.Scripts.PacsomanEditNodes
{
    public partial class ConfigPropertyNode<TIDType, T>(
        PacsomanEdit pacsomanEdit,
        string title,
        ConfigProperty<TIDType, T> configProperty
    ) : PacsomanEditNode(pacsomanEdit, title, false)
        where TIDType : notnull
        where T : IEquatable<T>
    {
        public readonly ConfigProperty<TIDType, T> ConfigProperty = configProperty;

        private readonly Label IDLabel = new();

        private readonly Label ValueLabel = new();

        public override void _EnterTree()
        {
            base._EnterTree();

            AddChild(IDLabel);
            IDLabel.SelfModulate = Colors.Gray;
            IDLabel.Text = $"{ConfigProperty.ID}:";

            AddChild(ValueLabel);
            ValueLabel.HorizontalAlignment = HorizontalAlignment.Right;
            ValueLabel.Text = ConfigProperty.Value.ToString();
            ConfigProperty.ValueChanged += OnValueChanged;

            SetTypedTitlebarColor<T>();
            CreateSlotRight<T>(0, ConfigProperty);
        }

        private void OnValueChanged()
        {
            ValueLabel.Text = ConfigProperty.Value.ToString();
        }
    }
}
