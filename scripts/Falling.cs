using Godot;
using System;

public partial class Falling : PlayerState {
    [Export] RayCast2D _onGroundRayCast;

    float _gravity, _terminalVelocity;

    public override void _Ready() {
        base._Ready();

        _gravity = (-2 * _player.JumpMaxHeight * MathF.Pow(_player.MovementSpeed, 2)) / MathF.Pow(_player.JumpFallDistance, 2);

        _terminalVelocity = -(2 * _player.JumpMaxHeight * _player.MovementSpeed) / _player.JumpFallDistance;
    }

    public override void Enter() {

    }

    public override void Update(float delta) {
        if (_onGroundRayCast.IsColliding()) {
            _stateMachine.ChangeState("OnGround");
            return;
        }

        if (Input.IsActionPressed("FastFall")) {
            _stateMachine.ChangeState("FastFall");
            return;
        }


        // 1 / delta = how many timesteps
        // speed / (1 / d) = convert from position per sec to position per timestep
        // the rest is just, accel = speed / time
        var horizontalAccel = (_player.MovementSpeed / (1 / delta)) / (_player.FramesToMaxSpeedInAir / 60);
        float dir = Input.GetAxis("Left", "Right");

        if (dir != 0) {
            _player.Velocity.X = Mathf.Clamp(_player.Velocity.X + dir * horizontalAccel, -_player.MovementSpeed, _player.MovementSpeed);
        } else if (_player.Velocity.X < 0) {
            _player.Velocity.X = MathF.Min(_player.Velocity.X + horizontalAccel, 0);
        } else if (_player.Velocity.X > 0) {
            _player.Velocity.X = MathF.Max(_player.Velocity.X - horizontalAccel, 0);
        }


        _player.Velocity.Y = MathF.Max(_player.Velocity.Y + _gravity * delta, _terminalVelocity);

        _player.MoveAndCollide(new Vector2(_player.Velocity.X, -_player.Velocity.Y) * delta);
    }
}
