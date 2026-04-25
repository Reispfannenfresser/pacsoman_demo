namespace Example.Scripts;

using Godot;
using Pacsoman;

public abstract partial class PacsomanEditNode(
    PacsomanEdit pacsomanEdit,
    string title,
    bool deletable = true
) : GraphNode
{
    public readonly PacsomanEdit PacsomanEdit = pacsomanEdit;

    private readonly Dictionary<int, IValueConsumer> _leftSlotOwners = [];
    private readonly Dictionary<int, IValueProvider> _rightSlotOwners = [];
    private readonly Dictionary<IValueConsumer, int> _leftSlotIndices = [];
    private readonly Dictionary<IValueProvider, int> _rightSlotIndices = [];
    public readonly bool Deletable = deletable;

    public override void _EnterTree()
    {
        base._EnterTree();
        Title = title;
    }

    protected void SetTitleBarColor(Color color)
    {
        StyleBoxFlat newStyleBoxTitle = (StyleBoxFlat)GetThemeStylebox("titlebar").Duplicate();
        StyleBoxFlat newStyleBoxTitleSelected = (StyleBoxFlat)
            GetThemeStylebox("titlebar_selected").Duplicate();
        newStyleBoxTitle.BgColor = new Color(color.R, color.G, color.B, 0.5f);
        newStyleBoxTitleSelected.BgColor = new Color(color.R, color.G, color.B, 0.75f);
        AddThemeStyleboxOverride("titlebar", newStyleBoxTitle);
        AddThemeStyleboxOverride("titlebar_selected", newStyleBoxTitleSelected);
    }

    protected void SetTypedTitlebarColor<T>()
        where T : IEquatable<T>
    {
        if (PacsomanEdit.TryGetTypeInfo<T>(out var typeInfo))
        {
            SetTitleBarColor(typeInfo!.Color);
        }
        else
        {
            GD.PrintErr($"Failed to set titlebar color: Unknown type ({typeof(T)})");
        }
    }

    public bool TryGetPortSlotRight(int portIndex, out int slotIndex)
    {
        foreach (int index in _rightSlotIndices.Values.Order())
        {
            if (IsSlotEnabledRight(index))
            {
                if (--portIndex == -1)
                {
                    slotIndex = index;
                    return true;
                }
            }
        }
        slotIndex = default;
        return false;
    }

    public bool TryGetPortSlotLeft(int portIndex, out int slotIndex)
    {
        foreach (int index in _leftSlotIndices.Values.Order())
        {
            if (IsSlotEnabledLeft(index))
            {
                if (--portIndex == -1)
                {
                    slotIndex = index;
                    return true;
                }
            }
        }
        slotIndex = default;
        return false;
    }

    public bool TryGetSlotIndexLeft(IValueConsumer owner, out int index)
    {
        return _leftSlotIndices.TryGetValue(owner, out index);
    }

    public bool TryGetSlotIndexRight(IValueProvider owner, out int index)
    {
        return _rightSlotIndices.TryGetValue(owner, out index);
    }

    public bool TryGetSlotOwnerLeft(int index, out IValueConsumer? owner)
    {
        return _leftSlotOwners.TryGetValue(index, out owner);
    }

    public bool TryGetSlotOwnerRight(int index, out IValueProvider? owner)
    {
        return _rightSlotOwners.TryGetValue(index, out owner);
    }

    public void CreateSlotLeft<T>(int index, IValueConsumer slotOwner)
        where T : IEquatable<T>
    {
        if (PacsomanEdit.TryGetTypeInfo<T>(out PacsomanEdit.TypeInfo<T>? typeInfo))
        {
            if (_leftSlotOwners.TryAdd(index, slotOwner))
            {
                if (_leftSlotIndices.TryAdd(slotOwner, index))
                {
                    SetSlotEnabledLeft(index, true);
                    SetSlotTypeLeft(index, typeInfo!.ID);
                    SetSlotColorLeft(index, typeInfo!.Color);
                }
                else
                {
                    GD.PrintErr($"Failed to create input slot: Owner {slotOwner} already used");
                    _leftSlotOwners.Remove(index);
                }
            }
            else
            {
                GD.PrintErr($"Failed to create input slot: Index {index} already used");
            }
        }
        else
        {
            GD.PrintErr($"Failed to create input slot: Unknown type ({typeof(T)})");
        }
    }

    public void CreateSlotRight<T>(int index, IValueProvider slotOwner)
        where T : IEquatable<T>
    {
        if (PacsomanEdit.TryGetTypeInfo<T>(out PacsomanEdit.TypeInfo<T>? typeInfo))
        {
            if (_rightSlotOwners.TryAdd(index, slotOwner))
            {
                if (_rightSlotIndices.TryAdd(slotOwner, index))
                {
                    SetSlotEnabledRight(index, true);
                    SetSlotTypeRight(index, typeInfo!.ID);
                    SetSlotColorRight(index, typeInfo!.Color);
                }
                else
                {
                    GD.PrintErr($"Failed to create output slot: Owner ({slotOwner}) already used");
                    _rightSlotOwners.Remove(index);
                }
            }
            else
            {
                GD.PrintErr($"Failed to create output slot: Index ({index}) already used");
            }
        }
        else
        {
            GD.PrintErr($"Failed to create output slot: Unknown type ({typeof(T)})");
        }
    }

    protected ValueEdit<T> AddInput<T>(int index, T initialValue)
        where T : IEquatable<T>
    {
        if (PacsomanEdit.TryGetTypeInfo<T>(out PacsomanEdit.TypeInfo<T>? typeInfo))
        {
            ValueEdit<T> valueEdit = PacsomanEdit.GetTypeInfo<T>().TEditCreator(initialValue);
            valueEdit.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            AddChild(valueEdit);
            CreateSlotLeft<T>(index, valueEdit);
            valueEdit.ValueChanged += OnInputChanged;
            return valueEdit;
        }
        else
        {
            throw new InvalidOperationException(
                $"Failed to create input: Unknown type ({typeof(T)})"
            );
        }
    }

    protected virtual void OnInputChanged() { }
}
