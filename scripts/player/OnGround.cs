using System;
using Godot;

public partial class OnGround : PlayerState {
    public override void Enter() {
        _player.Vel.Y = 0;
    }
    
    public override void Update(float delta) {
        HorizontalMovement(_player.FramesToMaxSpeed, delta);
        
        
        if (!_player.OnGround()) {
            _stateMachine.ChangeState("CoyoteTime");
            return;
        }

        if (Input.IsActionJustPressed("Jump")) {
            _stateMachine.ChangeState("Jump");
            return;
        }
    }
}
