using Godot;

namespace Example.Scripts;

public abstract partial class IndentedContent<TContentType>(int depth = 0) : HBoxContainer
    where TContentType : Control, new()
{
    public readonly int Depth = depth;

    private readonly Control Placeholder = new();

    protected readonly TContentType Content = new();

    public override void _EnterTree()
    {
        base._EnterTree();
        AddChild(Placeholder);
        Placeholder.CustomMinimumSize = Vector2.One * 16 * Depth;

        AddChild(Content);
        Content.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    }
}
