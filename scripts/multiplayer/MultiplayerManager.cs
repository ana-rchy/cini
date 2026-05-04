using Godot;

public partial class MultiplayerManager : Node {
    public override void _Ready() {
        GetTree().MultiplayerPoll = true;
    }
}
