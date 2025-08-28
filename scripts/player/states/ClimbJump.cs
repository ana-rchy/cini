using Godot;
using System;

public partial class ClimbJump : PlayerState {
    float _gravity;


    public override bool Prereqs() =>
        _player.OnWall() && _inputBuffer.ShouldJump() && _player.Stamina >= 0 &&
        Input.GetAxis("Left", "Right") != _player.GetWallNormal();

    public override void Enter() {
        _player.Vel.X = 0;

        _player.Vel.Y = GetJumpVelocity(
            _player.MovementSpeed,
            _player.WallJumpHeight,
            _player.WallJumpRiseDistance
        );

        _gravity = GetGravity(
            _player.MovementSpeed,
            _player.WallJumpHeight,
            _player.WallJumpRiseDistance
        );

        _player.TimeSinceJump = 0;
        _player.Stamina -= _player.ClimbJumpStaminaCost;

        _inputBuffer.StopJumpBufferTimer();
    }


    public override void Update(float delta) {
        HorizontalMovement(_player.MovementSpeed, _player.FramesToMaxSpeed, delta);

        float yAccel = GetVerletAccelTerm(_gravity, delta);
        float yPosChange = _player.Vel.Y * delta + yAccel;

        _player.MoveY(yPosChange);
        _player.Vel.Y += _gravity * delta;

        _player.TimeSinceJump += delta;
    }

    public override void Update_StateTransitions() {
        if (!Input.IsActionPressed("Jump")) {
            _stateMachine.ChangeState<ClimbShortJump>();
            return;
        }

        if (_player.Vel.Y >= 0) {
            if (_player.OnWall() && Input.IsActionPressed("Grab")) {
                _stateMachine.ChangeState<Climbing>();
            } else {
                _stateMachine.ChangeState<Falling>();
            }

            return;
        }

        if (!_player.OnWall() || _player.Stamina <= 0) {
            _stateMachine.ChangeState<Falling>();
            return;
        }
    }
}
