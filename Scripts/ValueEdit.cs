namespace Example.Scripts;

using Godot;
using Pacsoman;

public abstract partial class ValueEdit<T>(T initialValue)
    : HBoxContainer,
        IValueConsumer<T>,
        IValueProvider<T>
    where T : IEquatable<T>
{
    protected IValueProvider<T>? ValueProvider { get; private set; }

    protected T _internalValue = initialValue;

    public T InternalValue
    {
        get => _internalValue;
        protected set
        {
            _internalValue = value;
            InternalValueChanged();
            if (ValueProvider == null)
            {
                ValueChanged();
            }
        }
    }

    public event Action ValueChanged = delegate { };
    public event Action InternalValueChanged = delegate { };
    public event Action<IValueProvider?> ValueProviderChanged = delegate { };

    public override void _EnterTree()
    {
        SizeFlagsHorizontal = SizeFlags.ExpandFill;
    }

    public virtual T GetValue()
    {
        if (ValueProvider == null)
        {
            return InternalValue;
        }
        else
        {
            return ValueProvider.GetValue();
        }
    }

    public void SetValueProvider(IValueProvider<T>? provider)
    {
        if (ValueProvider != null)
        {
            ValueProvider.ValueChanged -= OnProvidedValueChanged;
        }
        ValueProvider = provider;
        if (ValueProvider != null)
        {
            ValueProvider.ValueChanged += OnProvidedValueChanged;
        }
        ValueProviderChanged(ValueProvider);
        ValueChanged();
    }

    public void SetValueProvider(IValueProvider? provider)
    {
        if (provider == null || provider is IValueProvider<T>)
        {
            SetValueProvider(provider as IValueProvider<T>);
        }
        else
        {
            throw new ArgumentException(
                $"The ValueProvider has an incompatible type: {typeof(IValueProvider<T>)} expected. Was {provider!.GetType()}"
            );
        }
    }

    private void OnProvidedValueChanged()
    {
        ValueChanged();
    }
}
