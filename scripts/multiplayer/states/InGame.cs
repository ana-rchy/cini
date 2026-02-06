using System.Collections.Generic;
using Godot;

public partial class InGame : State {
    [Export] Node _multiplayerManager;
    [Export] PackedScene _gameWorld;
    
    public override void Enter(Dictionary<string, object> dict = null) {
        Node world = _gameWorld.Instantiate();
        _multiplayerManager.AddChild(world);
    }
}
