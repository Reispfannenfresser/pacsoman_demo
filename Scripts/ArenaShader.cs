namespace Example.Scripts;

using System;
using Godot;
using Pacsoman;

public sealed partial class ArenaShader : ColorRect
{
    [Export]
    private Camera2D? _camera;

    [Export]
    private Arena? _arena;

    public override void _Ready()
    {
        base._Ready();
        if (Material is ShaderMaterial shaderMaterial)
        {
            Configuration.SpawnRadius.ActiveValueChanged += OnSpawnRadiusChanged;
            shaderMaterial.SetShaderParameter(
                "spawn_radius",
                Configuration.SpawnRadius.ActiveValue
            );
        }
    }

    private void OnSpawnRadiusChanged(Setting<string, float> setting, float arg2, float arg3)
    {
        if (Material is ShaderMaterial shaderMaterial)
        {
            shaderMaterial.SetShaderParameter(
                "spawn_radius",
                Configuration.SpawnRadius.ActiveValue
            );
        }
    }

    public override void _Process(double _)
    {
        if (Material is ShaderMaterial shaderMaterial)
        {
            if (_arena != null)
            {
                shaderMaterial.SetShaderParameter("border_radius", _arena.CurrentArenaSize);
            }
            if (_camera != null)
            {
                shaderMaterial.SetShaderParameter(
                    "camera_pos",
                    _camera.GlobalPosition + _camera.Offset
                );
                shaderMaterial.SetShaderParameter("camera_zoom", _camera.Zoom);
            }
        }
    }
}
