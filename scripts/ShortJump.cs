using Godot;
using System;

public partial class ShortJump : PlayerState {
    float _gravity;

    public override void _Ready() {
        base._Ready();

        _gravity = (-2 * _player.ShortJumpHeight * MathF.Pow(_player.MovementSpeed, 2)) / MathF.Pow(_player.ShortJumpRiseDistance, 2);
    }

    public override void Enter() {
        _player.Vel.Y = (2 * _player.ShortJumpHeight * _player.MovementSpeed) / _player.ShortJumpRiseDistance;

        // the principle is to convert the progress from a full jump into progress for a short jump
        //
        // progress in time and progress in distance are the same for the jump (assuming one direction is constantly held)
        //
        // so get the progress through the full jump as a float from 0-1, and lerp the velocity using that
        // (the velocity graph is a linear line so it works perfectly)
        float fullJumpProgress = (_player.JumpTimeBeforeShortJump * _player.MovementSpeed) / _player.JumpRiseDistance;

        _player.Vel.Y = Mathf.Lerp(_player.Vel.Y, 0, fullJumpProgress);
    }

    public override void Update(float delta) {
        if (Input.IsActionPressed("FastFall")) {
            _stateMachine.ChangeState("FastFall");
            return;
        }

        if (_player.Vel.Y <= 0) {
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

        // pos += vel * dt + (0.5 * accel * dt^2)
        // thats the original velocity verlet equation, but here its just dt, because move_and_slide also applies delta
        float accel_term = 0.5f * _gravity * delta;
        _player.Move(_player.Vel + new Vector2(0, accel_term));

        _player.Vel.Y += _gravity * delta;

        if (_player.IsOnFloor()) {
            _stateMachine.ChangeState("OnGround");
            return;
        }
    }
}
