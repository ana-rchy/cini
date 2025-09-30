using Godot;

// plan to remove
public partial class DebugState : PlayerState {
    float speed = 1f;
    
    public override void Update(float delta) {
        if (Input.IsActionJustPressed("Left")) {
            _player.MoveX(-speed);
        } else if (Input.IsActionJustPressed("Right")) {
            _player.MoveX(speed);
        }
        
        if (Input.IsActionJustPressed("Up")) {
            _player.MoveY(-speed);
        } else if (Input.IsActionJustPressed("Down")) {
            _player.MoveY(speed);
        }
    }
}
