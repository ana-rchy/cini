using Godot;
using System;

public partial class OnGround : PlayerState {
    [Export] Timer _jumpBufferTimer;


    public override void _Process(double delta) {
        if (Input.IsActionJustPressed("Jump")) {
            _jumpBufferTimer.Start();
        }
    }


    public override void Enter() {
        _player.Vel.Y = 0;

        if (!_jumpBufferTimer.IsStopped()) {
            _jumpBufferTimer.Stop();
            _stateMachine.ChangeState("Jump");
        }
    }

    public override void Update(float delta) {
        _player.Vel.X = _player.GetHorizontalVel(_player.FramesToMaxSpeed, delta);
        _player.Vel = _player.Move(_player.Vel);
        
        
        if (!_player.IsOnFloor()) {
            _stateMachine.ChangeState("CoyoteTime");
            return;
        }

        if (Input.IsActionJustPressed("Jump")) {
            _stateMachine.ChangeState("Jump");
            return;
        }
    }
}
