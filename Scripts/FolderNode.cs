using Godot;
using Pacsoman;

namespace Example.Scripts;

public partial class FolderNode<TIDType>(
    PacsomanEdit pacsomanEdit,
    string title,
    Folder<TIDType> folder
) : PacsomanEditNode(pacsomanEdit, title, false)
    where TIDType : notnull
{
    public readonly Folder<TIDType> Folder = folder;

    private readonly Dictionary<Folder<TIDType>, Header<TIDType>> _headers = [];

    public override void _EnterTree()
    {
        base._EnterTree();
        SetTitleBarColor(Colors.Black);
        AddSubfolders(Folder);
    }

    private void AddSubfolders(Folder<TIDType> folder, int depth = 0)
    {
        foreach (Folder<TIDType> subfolder in folder.GetSubfolders())
        {
            Header<TIDType> subfolderHeader = new(subfolder, depth);
            AddChild(subfolderHeader);
            _headers.Add(subfolder, subfolderHeader);
            AddSubfolders(subfolder, depth + 1);
        }
    }

    public void AddSettings<T>()
        where T : IEquatable<T>
    {
        AddSettingEdits<T>();
        AddSubfolderSettingEdits<T>();
    }

    private void AddSettingEdits<T>()
        where T : IEquatable<T>
    {
        foreach (Setting<TIDType, T> setting in Folder.GetSettings<T>())
        {
            AddSettingEdit<T>(setting);
        }
    }

    private void AddSubfolderSettingEdits<T>()
        where T : IEquatable<T>
    {
        foreach (Header<TIDType> header in _headers.Values)
        {
            foreach (Setting<TIDType, T> setting in header.Folder.GetSettings<T>())
            {
                AddSettingEdit<T>(setting, header.GetIndex(), header.Depth + 1);
            }
        }
    }

    private void AddSettingEdit<T>(Setting<TIDType, T> setting, int headerIndex = -1, int depth = 0)
        where T : IEquatable<T>
    {
        SettingEdit<TIDType, T> settingEdit = new(this, setting, depth);
        this.AddChild(settingEdit);
        this.MoveChild(settingEdit, headerIndex + 1);
    }
}
