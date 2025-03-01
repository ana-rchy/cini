using Godot;
using System;

public partial class FastFall : PlayerState {
    float _gravity, _terminalVelocity;

    public override void _Ready() {
        base._Ready();

        _gravity = (-2 * _player.JumpMaxHeight * MathF.Pow(_player.MovementSpeed, 2)) / MathF.Pow(_player.JumpFallDistance, 2);
        _gravity *= _player.FastFallMultiplier;

        _terminalVelocity = -(2 * _player.JumpMaxHeight * _player.MovementSpeed) / _player.JumpFallDistance;
        _terminalVelocity *= _player.FastFallMultiplier;
    }

    public override void Enter() {

    }

    public override void Update(float delta) {
        if (_player.IsOnFloor()) {
            _stateMachine.ChangeState("OnGround");
            return;
        }

        if (!Input.IsActionPressed("FastFall")) {
            _stateMachine.ChangeState("Falling");
            return;
        }


        // 1 / delta = how many timesteps
        // speed / (1 / d) = convert from position per sec to position per timestep
        // the rest is just, accel = speed / time
        var horizontalAccel = (_player.MovementSpeed / (1 / delta)) / (_player.FramesToMaxSpeedInAir / 60);
        float dir = Input.GetAxis("Left", "Right");

        if (dir != 0) {
            _player.Vel.X = Mathf.Clamp(_player.Vel.X + dir * horizontalAccel, -_player.MovementSpeed, _player.MovementSpeed);
        } else if (_player.Vel.X < 0) {
            _player.Vel.X = MathF.Min(_player.Vel.X + horizontalAccel, 0);
        } else if (_player.Vel.X > 0) {
            _player.Vel.X = MathF.Max(_player.Vel.X - horizontalAccel, 0);
        }

        _player.Vel.Y = MathF.Max(_player.Vel.Y + _gravity * delta, _terminalVelocity);

        _player.Move(new(_player.Vel.X, -_player.Vel.Y));
    }
}
