using Godot;
using System;

public partial class CoyoteTimeOld : PlayerStateOld {
    [Export] Timer _timer;


    public override void Enter() {
        _timer.Start(_player.CoyoteTime);
    }

    public override void Update(float delta) {
        _player.Vel.X = _player.GetHorizontalVel(_player.FramesToMaxSpeed, delta);
        _player.Vel = _player.Move(_player.Vel);
        
        
        if (_player.IsOnFloor()) {
            _stateMachine.ChangeState("OnGround");
            return;
        }

        if (!_player.IsOnFloor() && _timer.IsStopped()) {
            _stateMachine.ChangeState("Falling");
            return;
        }

        if (Input.IsActionJustPressed("Jump")) {
            _stateMachine.ChangeState("Jump");
            return;
        }
    }
}
