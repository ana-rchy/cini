using System.Collections.Generic;
using Godot;

public partial class SimpleRace : State {
    [Export] PackedScene _gameWorldScene, _puppetPlayerScene;
    [Export] Node _multiplayerManager;

    Player _player;
    
    Node _puppetPlayersHolder;
    Dictionary<long, PuppetPlayer> _puppetPlayers = new();



    public override void Enter(Dictionary<string, object> dict) {
        Multiplayer.PeerDisconnected += _OnPeerDisconnected;

        Node world = _gameWorldScene.Instantiate();
        _multiplayerManager.AddChild(world);
        
        _player = (Player) GetTree().GetFirstNodeInGroup("Player");
        
        _puppetPlayersHolder = new Node2D();
        world.AddChild(_puppetPlayersHolder);
        _puppetPlayersHolder.Name = "PuppetPlayers";
        

        List<long> players = (List<long>) dict["Players"];
        foreach (long id in players) {
            PuppetPlayer puppetPlayer = _puppetPlayerScene.Instantiate<PuppetPlayer>();
            _puppetPlayersHolder.AddChild(puppetPlayer);
            _puppetPlayers[id] = puppetPlayer;

            puppetPlayer.Name = $"PuppetPlayer-{id}";
        }
    }

    
    public override void Update(float _) {
        Rpc(MethodName.MovePuppetPlayer, _player.Pos.xInt, _player.Pos.yInt);
    }

    
    public override void Exit() {
        Multiplayer.PeerDisconnected -= _OnPeerDisconnected;
    }



    void _OnPeerDisconnected(long id) {
        Logging.Log($"{id} disconnected", Logging.LogCategory.Multiplayer);
        
        _puppetPlayers[id].QueueFree();
        _puppetPlayers.Remove(id);
    }
    

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.UnreliableOrdered)]
    void MovePuppetPlayer(int x, int y) {
        int senderID = Multiplayer.GetRemoteSenderId();
        PuppetPlayer puppetPlayer = _puppetPlayers.GetValueOrDefault(senderID, null);

        if (puppetPlayer == null) {
            return;
        }
        
        puppetPlayer.MoveTo(x, y);
    }
}
