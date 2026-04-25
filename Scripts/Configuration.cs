using Pacsoman;

namespace Example.Scripts;

public static class Configuration
{
    public static readonly Pacsoman<string> Pacsoman;

    public static readonly Folder<string> ArenaFolder;
    public static readonly Setting<string, float> BorderRadius;
    public static readonly Setting<string, float> SpawnRadius;

    public static readonly Folder<string> PlayerFolder;
    public static readonly Setting<string, float> MaxLinearVelocity;
    public static readonly Setting<string, float> AngularAcceleration;
    public static readonly Setting<string, float> ChargeMultiplier;
    public static readonly Setting<string, float> KillSpeed;

    public static readonly ConfigProperty<string, float> PlayerCount;

    static Configuration()
    {
        Pacsoman = new(new StringIDHandler("."));
        ArenaFolder = Pacsoman.AddFolder("Arena settings")!;
        BorderRadius = ArenaFolder.AddSetting<float>("Border radius", 700)!;
        SpawnRadius = ArenaFolder.AddSetting<float>("Spawn radius", 300)!;

        PlayerFolder = Pacsoman.AddFolder("Player settings")!;
        MaxLinearVelocity = PlayerFolder.AddSetting<float>("Max linear velocity", 2000)!;
        AngularAcceleration = PlayerFolder.AddSetting<float>("Angular acceleration", 20f)!;
        ChargeMultiplier = PlayerFolder.AddSetting<float>("Charge Multiplier", 2f)!;
        KillSpeed = PlayerFolder.AddSetting<float>("Kill Velocity", 500)!;

        PlayerCount = Pacsoman.AddConfigProperty<float>("Player count", 0);
    }
}
