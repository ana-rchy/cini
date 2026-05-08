using System.Threading.Tasks;
using Godot;

public partial class HostOrJoin : State {
    const string IP = "";
    const int PORT = 25565;
    const int MAX_PLAYERS = 10;

    [Export] Control _uiContainer;
    [Export] LineEdit _ipEntry;
    [Export] SpinBox _portEntry;


    public override void Exit() {
        _uiContainer.Hide();
    }


    void _OnHostPressed() {
        int port = (int) _portEntry.Value;

        ENetMultiplayerPeer peer = new();
        peer.CreateServer(port, MAX_PLAYERS - 1); // 2nd arg = how many *other* non-server clients can join
        Multiplayer.MultiplayerPeer = peer;

        Task.Run(() => UPNPSetup(port)); // upnp can take a bit

        _stateMachine.ChangeState<Freeplay>();
    }

    void _OnJoinPressed() {
        ENetMultiplayerPeer peer = new();
        peer.CreateClient(_ipEntry.Text, (int) _portEntry.Value);
        Multiplayer.MultiplayerPeer = peer;

        _stateMachine.ChangeState<Freeplay>();
    }
    

    void UPNPSetup(int port) {
        Upnp upnp = new();
        var error = upnp.Discover();

        switch (error) {
            case ((int) Upnp.UpnpResult.Success):
                Logging.Log("UPNP success", Logging.LogCategory.Multiplayer);
                break;

            default:
                Logging.Log($"UPNP error: {error}", Logging.LogCategory.Multiplayer);
                break;
        }

        if (upnp.GetGateway() != null && upnp.GetGateway().IsValidGateway()) {
            upnp.AddPortMapping(port, proto: "UDP");
            upnp.AddPortMapping(port, proto: "TCP");
        }
    }
}
