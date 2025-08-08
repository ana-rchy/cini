using Godot;
using System;

public partial class NeutralWallJump : PlayerState {
    float _gravity;


    public override void Enter() {
        _player.Vel.Y = (-2 * _player.NeutralWallJumpHeight * _player.MovementSpeed) / _player.NeutralWallJumpRiseDistance;

        _gravity = (2 * _player.WallJumpHeight * MathF.Pow(_player.MovementSpeed, 2))
                   / MathF.Pow(_player.NeutralWallJumpRiseDistance, 2);

        _player.Vel.X = _player.NeutralWallJumpPushoffSpeed * _player.GetWallNormal();

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
