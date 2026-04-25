using Godot;

namespace Example.Scripts;

public partial class Player : RigidBody2D
{
    [Export]
    private Sprite2D? Sprite;

    [Export]
    private Label? Label;

    [Export]
    private CollisionPolygon2D? Shape;

    [Export]
    private CpuParticles2D? Particles;

    public Key PlayerKey;

    private bool _clockwise = true;

    public bool Charged { get; private set; } = false;

    public Color Color { get; private set; } =
        new Color(
            (0.5f * GD.Randf()) + 0.5f,
            (0.5f * GD.Randf()) + 0.5f,
            (0.5f * GD.Randf()) + 0.5f,
            1f
        );

    public event Action<Player> Died = delegate { };

    public void SetKey(Key key)
    {
        PlayerKey = key;
        if (Label != null)
        {
            Label.Text = PlayerKey.ToString();
        }
    }

    public override void _Ready()
    {
        base._Ready();
        if (Sprite != null)
        {
            Sprite.SelfModulate = Color;
        }
        if (Particles != null)
        {
            Particles.Color = Color;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        float turnDirection = _clockwise ? 1 : -1;
        if (Input.IsKeyPressed(PlayerKey))
        {
            LinearVelocity = Vector2.Zero;
            AngularVelocity +=
                (float)delta * Configuration.AngularAcceleration.ActiveValue * turnDirection;
            Charged = true;
        }
        else if (Charged)
        {
            Charged = false;
            (float y, float x) = Mathf.SinCos(Rotation);
            LinearVelocity =
                Math.Min(
                    Configuration.MaxLinearVelocity.ActiveValue,
                    Mathf.RadToDeg(Mathf.Abs(AngularVelocity))
                        * Configuration.ChargeMultiplier.ActiveValue
                ) * new Vector2(x, y);
            AngularVelocity = 0;
            _clockwise = !_clockwise;
        }
    }

    public void Kill()
    {
        Died(this);
        QueueFree();
    }

    public bool IsPointInside(Vector2 point)
    {
        point = ToLocal(point);
        if (Shape != null)
        {
            return Geometry2D.IsPointInPolygon(point, Shape.Polygon);
        }
        return false;
    }

    public Vector2 GetNosePos()
    {
        return ToGlobal(new Vector2(171, 0));
    }
}
