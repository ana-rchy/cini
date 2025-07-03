using System;
using Godot;

public partial class OnGround : PlayerState {
    public override void Enter() {
        _player.Vel.Y = 0;
    }
    
    public override void Update(float delta) {
        _player.Vel.X = _player.GetMovementXVel(_player.Vel.X, _player.FramesToMaxSpeed, delta);
        _player.MoveX(_player.Vel.X * delta);
        
        
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
