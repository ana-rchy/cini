using Godot;

public partial class PixelCameraStateMachine : StateMachine {
    public override void _PhysicsProcess(double _) {}
    
    public override void _Process(double delta) {
        base._PhysicsProcess(delta);
    }
}
