using System.Collections.Generic;
using Godot;

public partial class InGame : State {
    [Export] Node _multiplayerManager;
    [Export] PackedScene _gameWorld;
    
    public override void Enter(Dictionary<string, object> dict = null) {
        Node world = _gameWorld.Instantiate();
        _multiplayerManager.AddChild(world);

        Multiplayer.PeerConnected += _OnPeerConnected;
    }

    
    public override void Exit() {
        Multiplayer.PeerConnected -= _OnPeerConnected;
    }


    void _OnPeerConnected(long id) {
        GD.Print(id);
    }
}
