namespace Example.Scripts;

using Godot;
using PacsomanEditNodes;

public partial class NodeCreationButton(
    PacsomanEdit pacsomanEdit,
    Texture2D? icon,
    Vector2 positionOffset,
    Func<PacsomanEditNode> creationFunction
) : Button
{
    public readonly PacsomanEdit PacsomanEdit = pacsomanEdit;

    protected readonly Func<PacsomanEditNode> CreateNode = creationFunction;

    protected readonly Vector2 PositionOffset = positionOffset;

    public override void _Ready()
    {
        base._Ready();
        Icon = icon;
        Pressed += OnPressed;
    }

    public virtual void OnPressed()
    {
        PacsomanEditNode node = CreateNode();
        node.PositionOffset = PositionOffset;
        PacsomanEdit.AddChild(node);
    }
}
