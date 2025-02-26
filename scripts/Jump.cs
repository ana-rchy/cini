using Godot;
using System;

public partial class Jump : PlayerState {
    [Export] RayCast2D _onGroundRayCast;
    [Export] Timer _jumpArcTimer;

    float _groundYPosition;

    public override void Enter() {
        _jumpArcTimer.Start(_player.JumpTime);
        _groundYPosition = _player.Position.Y;
    }

    public override void Update(double delta) {
        if (_onGroundRayCast.IsColliding()) {
            _stateMachine.ChangeState("OnGround");
            return;
        }

        if (_jumpArcTimer.TimeLeft == 0) {
            _stateMachine.ChangeState("Falling");
            return;
        }

        if (Input.IsActionPressed("FastFall")) {
            _stateMachine.ChangeState("FastFall");
            return;
        }

        var yVelocityTarget = _player.JumpArc.SampleBaked(1 - ((float) _jumpArcTimer.TimeLeft / _player.JumpTime)) * -_player.JumpHeight;
        var yVelocity = yVelocityTarget - (_player.Position.Y - _groundYPosition);

        _player.MoveAndCollide(new(0, yVelocity));
        _player.VelocityPerSec = new(0, yVelocity * (1 / (float) delta));

        GD.Print(_player.VelocityPerSec);
    }
}
