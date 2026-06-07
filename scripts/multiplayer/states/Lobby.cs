using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Lobby : State {
    Dictionary<long, bool> _playersReady;



    public override void Enter() {
        Multiplayer.PeerConnected += _OnPeerConnected;
        Multiplayer.PeerDisconnected += _OnPeerDisconnected;
        
        _playersReady = new() { { Multiplayer.GetUniqueId(), false } };
    }

    
    public override void Update(float _) {
        if (Input.IsActionJustPressed("Dash")) {
            Rpc(MethodName.ChangeReady, !_playersReady[Multiplayer.GetUniqueId()]);
        }

        if (_playersReady.Count > 1 && _playersReady.Values.All(ready => ready == true)) {
            List<long> players = _playersReady.Keys.ToList();

            _stateMachine.ChangeState<SimpleRace>(dict: new(){ { "Players", players } });
        }
    }


    public override void Exit() {
        Multiplayer.PeerConnected -= _OnPeerConnected;
        Multiplayer.PeerDisconnected -= _OnPeerDisconnected;
    }



    void _OnPeerConnected(long id) {
        Logging.Log($"{id} connected", Logging.LogCategory.Multiplayer);
        
        _playersReady.Add(id, false);
    }

    void _OnPeerDisconnected(long id) {
        Logging.Log($"{id} disconnected", Logging.LogCategory.Multiplayer);

        Logging.Assert(_playersReady.ContainsKey(id), $"{nameof(_playersReady)} missing key {id}");
        _playersReady.Remove(id);
    }
    

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    void ChangeReady(bool ready) {
        int senderID = Multiplayer.GetRemoteSenderId();
        _playersReady[senderID] = ready;
    }
}
