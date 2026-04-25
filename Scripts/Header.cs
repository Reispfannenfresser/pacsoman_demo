using Godot;
using Pacsoman;

namespace Example.Scripts;

public partial class Header<TIDType>(Folder<TIDType> folder, int depth = 0)
    : IndentedContent<Label>(depth)
    where TIDType : notnull
{
    public readonly Folder<TIDType> Folder = folder;

    public override void _Ready()
    {
        base._Ready();
        Content.Text = $"{Folder.ID}:";
        Content.HorizontalAlignment = HorizontalAlignment.Left;
    }
}
