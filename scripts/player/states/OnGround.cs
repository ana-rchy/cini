using System;
using Godot;

public partial class OnGround : PlayerState {
    public override bool Prereqs() {
        if (_player.OnGround()) {
            return true;
        }

        return false;
    }

    public override void Enter() {
        _player.Vel.Y = 0;
        _player.Stamina = _player.TotalStamina;
        _player.DashCharges = _player.DashChargeTotal;
    }

    public override void Update(float delta) {
        HorizontalMovement(_player.FramesToMaxSpeed, delta);


        if (!_player.OnGround()) {
            _stateMachine.ChangeState("CoyoteTime");
            return;
        }

        if (Input.IsActionJustPressed("Jump") || _inputBuffer.ShouldJump) {
            _stateMachine.ChangeState("Jump");
            _inputBuffer.StopJumpBufferTimer();
            return;
        }

        if (_player.OnWall() && Input.IsActionPressed("Grab")) {
            _stateMachine.ChangeState("Climbing");
            return;
        }

        if (Input.IsActionJustPressed("Dash")) {
            _stateMachine.ChangeState("Dashing");
            return;
        }
    }
}
