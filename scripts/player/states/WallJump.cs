using Godot;
using System;

public partial class WallJump : PlayerState {
    [Export] Timer _pushoffTimer;
    float _gravity;


    public override void Enter() {
        _player.Vel.Y = (-2 * _player.WallJumpHeight * _player.MovementSpeed) / _player.WallJumpRiseDistance;

        _gravity = (2 * _player.WallJumpHeight * MathF.Pow(_player.MovementSpeed, 2))
                   / MathF.Pow(_player.WallJumpRiseDistance, 2);

        _player.Vel.X = _player.WallJumpPushoffSpeed * _player.GetWallNormal();

        _player.TimeSinceJump = 0;

        _pushoffTimer.OneShot = true;
        _pushoffTimer.Start(_player.WallJumpPushoffTime);
    }

    public override void Update(float delta) {
        GD.Print(_player.Vel.X);
        _player.MoveX(_player.Vel.X * delta);

        float accelTerm = GetVerletAccelTerm(_gravity, delta);
        float yPosChange = _player.Vel.Y * delta + accelTerm;

        _player.MoveY(yPosChange);
        _player.Vel.Y += _gravity * delta;

        WallClip();

        _player.TimeSinceJump += delta;


        CommonInAirTransitions();

        if (_pushoffTimer.IsStopped()) {
            _stateMachine.ChangeState("Falling");
            return;
        }

        if (!Input.IsActionPressed("Jump")) {
            _stateMachine.ChangeState("ShortJump");
            return;
        }

        if (!Input.IsActionPressed("Jump")) {
            _stateMachine.ChangeState("ShortJump");
            return;
        }
    }
}
