using System;
using Godot;

public partial class OnGround : PlayerState {
    public override bool Prereqs() =>
        _player.OnGround();

    public override void Enter() {
        _player.Vel.Y = 0;
        _player.Stamina = _player.TotalStamina;
        _player.DashCharges = _player.DashChargeTotal;
    }
    

    public override void Update(float delta) {
        BasicXMovement(_player.MovementSpeed, _player.FramesToMaxSpeed, delta);
    }
    
    public override void Update_StateTransitions(float _) {
        if (!_player.OnGround()) {
            _stateMachine.ChangeState<CoyoteTime>();
            return;
        }

        if (_inputBuffer.ShouldJump()) {
            _stateMachine.ChangeState<Jump>();
            return;
        }

        if (_player.OnWall() && Input.IsActionPressed("Grab")) {
            _stateMachine.ChangeState<Climbing>();
            return;
        }

        if (Input.IsActionJustPressed("Dash")) {
            _stateMachine.ChangeState<Dash>();
            return;
        }
    }
}
