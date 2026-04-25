using Example.Scripts;
using Godot;

public partial class CameraController : Camera2D
{
    [Export]
    private float _padding = 20f;

    private double _desiredZoom = 0f;

    [Export]
    private double Decay = 2f;

    public override void _Ready()
    {
        base._Ready();
        Configuration.BorderRadius.ActiveValueChanged += OnArenaSizeChanged;
        SetFrame();
        GetTree().Root.SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged()
    {
        SetFrame();
    }

    private void SetFrame()
    {
        Vector2 viewportSize = GetViewportRect().Size;
        float maxAvailableSpace = Math.Min(viewportSize.X, viewportSize.Y);
        float requiredSpace = (Configuration.BorderRadius.ActiveValue * 2) + _padding;
        _desiredZoom = maxAvailableSpace / requiredSpace;
    }

    public void OnArenaSizeChanged(
        Pacsoman.Setting<string, float> setting,
        float oldValue,
        float newValue
    )
    {
        SetFrame();
    }

    public override void _Process(double delta)
    {
        Zoom = Vector2.One * (float)Interpolation.ExpDecay(Zoom.X, _desiredZoom, Decay, delta);
    }
}
