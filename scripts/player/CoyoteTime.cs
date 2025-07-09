using Godot;
using System;

public partial class CoyoteTime : PlayerState {
    [Export] Timer _timer;


    public override void Enter() {
        _timer.OneShot = true;
        _timer.Start(_player.CoyoteTime);
    }

    public override void Update(float delta) {
        HorizontalMovement(_player.FramesToMaxSpeed, delta);
        
        
        if (_player.OnGround()) {
            _stateMachine.ChangeState("OnGround");
            return;
        }

        if (!_player.OnGround() && _timer.IsStopped()) {
            _stateMachine.ChangeState("Falling");
            return;
        }

        if (Input.IsActionJustPressed("Jump")) {
            _stateMachine.ChangeState("Jump");
            return;
        }
    }
}
