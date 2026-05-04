using Godot;

public partial class HostOrJoin : State {
    const string IP = "localhost";
    const int PORT = 25565;
    const int MAX_PLAYERS = 10;

    public override void Update(float _) {
        // HACK: just to test multiplayer quickly
        if (Input.IsActionJustPressed("Grab")) {
            // server
            ENetMultiplayerPeer peer = new();
            peer.CreateServer(PORT, MAX_PLAYERS - 1); // 2nd arg = how many *other* non-server clients can join
            Multiplayer.MultiplayerPeer = peer;

            _stateMachine.ChangeState<Freeplay>();
            return;

        } else if (Input.IsActionJustPressed("Jump")) {
            // client
            ENetMultiplayerPeer peer = new();
            peer.CreateClient(IP, PORT);
            Multiplayer.MultiplayerPeer = peer;
            
            _stateMachine.ChangeState<Freeplay>();
            return;
        }
    }
}
