using Example.Scripts.PacsomanEditNodes;
using Example.Scripts.ValueEdits;
using Godot;
using Pacsoman;

namespace Example.Scripts;

public partial class PacsomanEdit : GraphEdit
{
    [Export]
    public Control? NodeButtonContainer = null;

    public delegate ValueEdit<T> TEditCreator<T>(T defaultValue)
        where T : IEquatable<T>;

    public sealed class TypeInfo<T>(int id, Color color, TEditCreator<T> tEditCreator)
        where T : IEquatable<T>
    {
        public readonly int ID = id;
        public readonly Color Color = color;
        public readonly TEditCreator<T> TEditCreator = tEditCreator;
    }

    private static class PacsomanTypes<T>
        where T : IEquatable<T>
    {
        public static Dictionary<PacsomanEdit, TypeInfo<T>> Infos = [];
    }

    private int _typeCount = 0;

    public event Action TypesRegistered = delegate { };

    private readonly Dictionary<Folder<string>, FolderNode<string>> _folderNodes = [];

    private readonly Dictionary<IValueProvider, PacsomanEditNode> _configPropertyNodes = [];

    public override void _EnterTree()
    {
        base._EnterTree();

        AddNodeButtons();
        AddFolders();
        RegisterTypes();

        ConnectionRequest += OnConnectionRequest;
        DisconnectionRequest += OnDisconnectionRequest;
        DeleteNodesRequest += OnDeleteNodesRequest;
    }

    private void AddNodeButtons()
    {
        Vector2 offset = Vector2.One * 100;
        NodeButtonContainer?.AddChild(
            new NodeCreationButton(
                this,
                ResourceLoader.Load<Texture2D>("Icons/Add.png"),
                offset,
                () => new AdditionNode(this)
            )
        );
        NodeButtonContainer?.AddChild(
            new NodeCreationButton(
                this,
                ResourceLoader.Load<Texture2D>("Icons/Subtract.png"),
                offset,
                () => new SubtractionNode(this)
            )
        );
        NodeButtonContainer?.AddChild(
            new NodeCreationButton(
                this,
                ResourceLoader.Load<Texture2D>("Icons/Multiply.png"),
                offset,
                () => new MultiplicationNode(this)
            )
        );
        NodeButtonContainer?.AddChild(
            new NodeCreationButton(
                this,
                ResourceLoader.Load<Texture2D>("Icons/Divide.png"),
                offset,
                () => new DivisionNode(this)
            )
        );
        NodeButtonContainer?.AddChild(
            new NodeCreationButton(
                this,
                ResourceLoader.Load<Texture2D>("Icons/Maximum.png"),
                offset,
                () => new MaximumNode(this)
            )
        );
        NodeButtonContainer?.AddChild(
            new NodeCreationButton(
                this,
                ResourceLoader.Load<Texture2D>("Icons/Minimum.png"),
                offset,
                () => new MinimumNode(this)
            )
        );
        NodeButtonContainer?.AddChild(
            new NodeCreationButton(
                this,
                ResourceLoader.Load<Texture2D>("Icons/Absolute.png"),
                offset,
                () => new AbsoluteNode(this)
            )
        );
        NodeButtonContainer?.AddChild(
            new NodeCreationButton(
                this,
                ResourceLoader.Load<Texture2D>("Icons/Sign.png"),
                offset,
                () => new SignNode(this)
            )
        );
        NodeButtonContainer?.AddChild(
            new NodeCreationButton(
                this,
                ResourceLoader.Load<Texture2D>("Icons/Greater.png"),
                offset,
                () => new GreaterNode(this)
            )
        );
        NodeButtonContainer?.AddChild(
            new NodeCreationButton(
                this,
                ResourceLoader.Load<Texture2D>("Icons/Lesser.png"),
                offset,
                () => new LesserNode(this)
            )
        );
        NodeButtonContainer?.AddChild(
            new NodeCreationButton(
                this,
                ResourceLoader.Load<Texture2D>("Icons/GreaterEqual.png"),
                offset,
                () => new GreaterEqualNode(this)
            )
        );
        NodeButtonContainer?.AddChild(
            new NodeCreationButton(
                this,
                ResourceLoader.Load<Texture2D>("Icons/LesserEqual.png"),
                offset,
                () => new LesserEqualNode(this)
            )
        );
        NodeButtonContainer?.AddChild(
            new NodeCreationButton(
                this,
                ResourceLoader.Load<Texture2D>("Icons/Equal.png"),
                offset,
                () => new EqualNode(this)
            )
        );
        NodeButtonContainer?.AddChild(
            new NodeCreationButton(
                this,
                ResourceLoader.Load<Texture2D>("Icons/Branch.png"),
                offset,
                () => new BranchNode(this)
            )
        );
    }

    public override void _Ready()
    {
        base._Ready();
        CreateInitialSetup();
        ArrangeInitialSetup(new Vector2(150, 150));
    }

    public void CreateInitialSetup()
    {
        MultiplicationNode multiplicationNode = new(this, 0, 75);
        AdditionNode additionNode = new(this, 0, 700);

        AddChild(multiplicationNode);
        AddChild(additionNode);

        if (
            _configPropertyNodes.TryGetValue(
                Configuration.PlayerCount,
                out PacsomanEditNode? playerCountNode
            )
        )
        {
            EmitSignalConnectionRequest(playerCountNode.Name, 0, multiplicationNode.Name, 0);
        }
        else
        {
            GD.PrintErr("Unable to locate ConfigPropertyNode of PlayerCount ConfigProperty");
        }

        EmitSignalConnectionRequest(multiplicationNode.Name, 0, additionNode.Name, 0);

        if (
            _folderNodes.TryGetValue(Configuration.ArenaFolder, out FolderNode<string>? arenaFolder)
        )
        {
            EmitSignalConnectionRequest(additionNode.Name, 0, arenaFolder.Name, 1);
        }
        else
        {
            GD.PrintErr("Unable to locate SettingEdit of BorderRadius Setting");
        }
    }

    private void ArrangeInitialSetup(Vector2 offset)
    {
        foreach (Node child in GetChildren())
        {
            if (child is PacsomanEditNode editNode)
            {
                editNode.Selected = true;
            }
        }
        ArrangeNodes();

        foreach (Node child in GetChildren())
        {
            if (child is PacsomanEditNode editNode)
            {
                editNode.Selected = false;
                editNode.PositionOffset += offset;
            }
        }

        if (
            _folderNodes.TryGetValue(
                Configuration.PlayerFolder,
                out FolderNode<string>? playerFolder
            )
            && _folderNodes.TryGetValue(
                Configuration.ArenaFolder,
                out FolderNode<string>? arenaFolder
            )
        )
        {
            playerFolder.PositionOffset = new Vector2(
                arenaFolder.PositionOffset.X,
                playerFolder.PositionOffset.Y - 50f
            );
        }
    }

    private void AddFolders()
    {
        foreach (Folder<string> folder in Configuration.Pacsoman.GetFolders())
        {
            FolderNode<string> folderNode = new(this, folder.ID, folder);
            _folderNodes.Add(folder, folderNode);
            AddChild(folderNode);
        }
    }

    private void EmitNodeDisconnectionRequest(StringName node)
    {
        foreach (var connection in Connections)
        {
            if ((string)connection["to_node"] == node)
            {
                EmitSignalDisconnectionRequest(
                    (string)connection["from_node"],
                    (long)connection["from_port"],
                    node,
                    (long)connection["to_port"]
                );
            }
            if ((string)connection["from_node"] == node)
            {
                EmitSignalDisconnectionRequest(
                    node,
                    (long)connection["from_port"],
                    (string)connection["to_node"],
                    (long)connection["to_port"]
                );
            }
        }
    }

    private void OnDeleteNodesRequest(Godot.Collections.Array<StringName> nodes)
    {
        foreach (StringName nodeName in nodes)
        {
            PacsomanEditNode? node = GetNode<PacsomanEditNode>($"{GetPath()}/{nodeName}");
            if (node != null && node.Deletable)
            {
                EmitNodeDisconnectionRequest(nodeName);
                node.QueueFree();
            }
        }
    }

    private void OnConnectionRequest(
        StringName fromNodeName,
        long fromPort,
        StringName toNodeName,
        long toPort
    )
    {
        PacsomanEditNode? fromNode = GetNode<PacsomanEditNode>($"{GetPath()}/{fromNodeName}");
        PacsomanEditNode? toNode = GetNode<PacsomanEditNode>($"{GetPath()}/{toNodeName}");
        if (
            fromNode != null
            && toNode != null
            && fromNode.TryGetPortSlotRight((int)fromPort, out int fromSlot)
            && fromNode.TryGetSlotOwnerRight(fromSlot, out IValueProvider? provider)
            && toNode.TryGetPortSlotLeft((int)toPort, out int toSlot)
            && toNode.TryGetSlotOwnerLeft(toSlot, out IValueConsumer? consumer)
        )
        {
            foreach (var connection in Connections)
            {
                if (
                    (string)connection["to_node"] == toNodeName
                    && (long)connection["to_port"] == toPort
                )
                {
                    EmitSignalDisconnectionRequest(
                        (string)connection["from_node"],
                        (long)connection["from_port"],
                        toNodeName,
                        toPort
                    );
                }
            }

            ConnectNode(fromNodeName, (int)fromPort, toNodeName, (int)toPort);
            consumer!.SetValueProvider(provider);
        }
    }

    private void OnDisconnectionRequest(
        StringName fromNodeName,
        long fromPort,
        StringName toNodeName,
        long toPort
    )
    {
        PacsomanEditNode fromNode = GetNode<PacsomanEditNode>($"{GetPath()}/{fromNodeName}");
        PacsomanEditNode toNode = GetNode<PacsomanEditNode>($"{GetPath()}/{toNodeName}");
        if (
            fromNode.TryGetPortSlotRight((int)fromPort, out int fromSlot)
            && fromNode.TryGetSlotOwnerRight(fromSlot, out IValueProvider? _)
            && toNode.TryGetPortSlotLeft((int)toPort, out int toSlot)
            && toNode.TryGetSlotOwnerLeft(toSlot, out IValueConsumer? consumer)
        )
        {
            DisconnectNode(fromNodeName, (int)fromPort, toNodeName, (int)toPort);
            consumer!.SetValueProvider(null);
        }
    }

    public void ChangeProvider(PacsomanEditNode node, int portIndex, IValueProvider newProvider)
    {
        foreach (var connection in Connections)
        {
            if (
                (string)connection["from_node"] == node.Name
                && (long)connection["from_port"] == portIndex
            )
            {
                PacsomanEditNode toNode = GetNode<PacsomanEditNode>(
                    $"{GetPath()}/{connection["to_node"]}"
                );

                if (
                    toNode.TryGetPortSlotLeft((int)connection["to_port"], out int index)
                    && toNode.TryGetSlotOwnerLeft(index, out IValueConsumer? consumer)
                )
                {
                    consumer!.SetValueProvider(newProvider);
                }
            }
        }
    }

    private void RegisterTypes()
    {
        RegisterType<float>(Colors.Blue, v => new FloatEdit(v));
        RegisterType<bool>(Colors.Red, v => new BoolEdit(v));
    }

    private void AddConfigProperties<T>()
        where T : IEquatable<T>
    {
        foreach (
            ConfigProperty<
                string,
                T
            > configProperty in Configuration.Pacsoman.GetConfigProperties<T>()
        )
        {
            ConfigPropertyNode<string, T> configPropertyNode = new(
                this,
                configProperty.ID,
                configProperty
            );
            _configPropertyNodes.Add(configProperty, configPropertyNode);
            AddChild(configPropertyNode);
        }
    }

    private void AddSettings<T>()
        where T : IEquatable<T>
    {
        foreach (FolderNode<string> folderNode in _folderNodes.Values)
        {
            folderNode.AddSettings<T>();
        }
    }

    public void RegisterType<T>(Color color, TEditCreator<T> tEditCreator)
        where T : IEquatable<T>
    {
        if (PacsomanTypes<T>.Infos.ContainsKey(this))
        {
            return;
        }
        PacsomanTypes<T>.Infos.Add(this, new TypeInfo<T>(_typeCount++, color, tEditCreator));
        AddSettings<T>();
        AddConfigProperties<T>();
    }

    public bool TryGetTypeInfo<T>(out TypeInfo<T>? typeInfo)
        where T : IEquatable<T>
    {
        return PacsomanTypes<T>.Infos.TryGetValue(this, out typeInfo);
    }

    public TypeInfo<T> GetTypeInfo<T>()
        where T : IEquatable<T>
    {
        return PacsomanTypes<T>.Infos[this];
    }
}
