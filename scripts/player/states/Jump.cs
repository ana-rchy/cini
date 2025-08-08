using Godot;
using System;

public partial class Jump : PlayerState {
    float _gravity;


    public override void Enter() {
        _player.Vel.Y = (-2 * _player.JumpHeight * _player.MovementSpeed) / _player.JumpRiseDistance;

        _gravity = (2 * _player.JumpHeight * MathF.Pow(_player.MovementSpeed, 2))
                   / MathF.Pow(_player.JumpRiseDistance, 2);

        _player.TimeSinceJump = 0;
    }

    public override void Update(float delta) {
        HorizontalMovement(_player.FramesToMaxSpeedInAir, delta);

        float accelTerm = GetVerletAccelTerm(_gravity, delta);
        float yPosChange = _player.Vel.Y * delta + accelTerm;

        _player.MoveY(yPosChange);
        _player.Vel.Y += _gravity * delta;

        WallClip();

        _player.TimeSinceJump += delta;


        if (CommonInAirTransitions()) return;

        if (Input.IsActionPressed("FastFall")) {
            _stateMachine.ChangeState("FastFall");
            return;
        }

        if (_player.Vel.Y >= 0) {
            _stateMachine.ChangeState("Falling");
            return;
        }

        if (!Input.IsActionPressed("Jump")) {
            _stateMachine.ChangeState("ShortJump");
            return;
        }
    }
}
