using Godot;
using Pacsoman;

namespace Example.Scripts;

public partial class Arena : Node2D
{
    private readonly Dictionary<Key, Player> _players = [];

    [Export]
    private PackedScene? _playerScene;

    private double _desiredArenaSize;

    public float CurrentArenaSize { get; private set; } = 0;

    [Export]
    private double _decay = 2;

    public override void _Ready()
    {
        base._Ready();
        CurrentArenaSize = Configuration.BorderRadius.ActiveValue;
        _desiredArenaSize = CurrentArenaSize;
        Configuration.BorderRadius.ActiveValueChanged += OnBorderRadiusChanged;
    }

    private void OnBorderRadiusChanged(
        Setting<string, float> setting,
        float oldValue,
        float newValue
    )
    {
        if (float.IsInfinity(oldValue) || float.IsNaN(oldValue))
        {
            CurrentArenaSize = newValue;
        }
        _desiredArenaSize = newValue;
    }

    public void PlayerDied(Player player)
    {
        _players.Remove(player.PlayerKey);
        Configuration.PlayerCount.Value = _players.Count;
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (
            @event is InputEventKey keyEvent
            && keyEvent.Keycode.ToString().Length == 1
            && !_players.ContainsKey(keyEvent.Keycode)
        )
        {
            AddPlayer(keyEvent.Keycode);
        }
    }

    private void AddPlayer(Key key)
    {
        Player player = (_playerScene?.Instantiate() as Player)!;
        player.SetKey(key);
        player.Died += PlayerDied;
        _players.Add(key, player);
        AddChild(player);
        Configuration.PlayerCount.Value = _players.Count;
    }

    public override void _Process(double delta)
    {
        if (!float.IsNaN(CurrentArenaSize))
        {
            CurrentArenaSize = (float)
                Interpolation.ExpDecay(CurrentArenaSize, _desiredArenaSize, _decay, delta);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        HandleArenaBoundsCollisions(delta);
        HandleSpawnBoundsCollisions(delta);
        HandlePlayerCollisions(delta);
    }

    private void HandleSpawnBoundsCollisions(double _)
    {
        float spawnSize = Configuration.SpawnRadius.ActiveValue;
        foreach (Player player in _players.Values)
        {
            if (player.Charged)
            {
                continue;
            }

            if (player.Position.Length() < spawnSize)
            {
                if (player.GlobalPosition == Vector2.Zero)
                {
                    continue;
                }
                if (player.Position.Dot(player.LinearVelocity) > 0)
                {
                    continue;
                }
                Vector2 Direction = player.LinearVelocity.Normalized();
                Vector2 BounceDirection = Direction.Bounce(player.GlobalPosition.Normalized());
                player.LinearVelocity = BounceDirection * player.LinearVelocity.Length();
                player.Rotation = Mathf.Atan2(player.LinearVelocity.Y, player.LinearVelocity.X);
                player.LookAt(player.GlobalPosition + BounceDirection);
            }
        }
    }

    private void HandleArenaBoundsCollisions(double _)
    {
        foreach (Player player in _players.Values)
        {
            if (player.Charged)
            {
                continue;
            }

            if (player.Position.Length() > CurrentArenaSize)
            {
                player.MoveAndCollide(
                    (player.Position.Normalized() * CurrentArenaSize) - player.GlobalPosition
                );
                if (player.Position.Dot(player.LinearVelocity) < 0)
                {
                    continue;
                }
                Vector2 Direction = player.LinearVelocity.Normalized();
                Vector2 BounceDirection = Direction.Bounce(player.GlobalPosition.Normalized());
                player.LinearVelocity = BounceDirection * player.LinearVelocity.Length();
                player.Rotation = Mathf.Atan2(player.LinearVelocity.Y, player.LinearVelocity.X);
                player.LookAt(player.GlobalPosition + BounceDirection);
            }
        }
    }

    private void HandlePlayerCollisions(double _)
    {
        Stack<Player> toKill = [];

        foreach (Player firstPlayer in _players.Values)
        {
            if (firstPlayer.Charged)
            {
                continue;
            }
            foreach (Player secondPlayer in _players.Values)
            {
                if (firstPlayer == secondPlayer)
                {
                    continue;
                }
                if (
                    (
                        firstPlayer.ToLocal(firstPlayer.LinearVelocity + firstPlayer.GlobalPosition)
                        - firstPlayer.ToLocal(
                            secondPlayer.LinearVelocity + firstPlayer.GlobalPosition
                        )
                    ).X < Configuration.KillSpeed.ActiveValue
                )
                {
                    continue;
                }
                if (secondPlayer.IsPointInside(firstPlayer.GetNosePos()))
                {
                    toKill.Push(secondPlayer);
                }
            }
        }

        while (toKill.TryPop(out Player? player))
        {
            player.Kill();
        }
    }
}
