using Godot;
using System;

public partial class OnGround : PlayerState {
    public override void Enter() {
        _player.Vel.Y = 0;
    }

    public override void Update(float delta) {
        if (!_player.IsOnFloor()) {
            _stateMachine.ChangeState("Falling");
            return;
        }

        if (Input.IsActionJustPressed("Jump")) {
            _stateMachine.ChangeState("Jump");
            return;
        }


        // 1 / delta = how many timesteps
        // speed / (1 / d) = convert from position per sec to position per timestep
        // the rest is just, accel = speed / time
        var horizontalAccel = (_player.MovementSpeed / (1 / delta)) / (_player.FramesToMaxSpeed / 60);
        float dir = Input.GetAxis("Left", "Right");

        if (dir != 0) {
            _player.Vel.X = Mathf.Clamp(_player.Vel.X + dir * horizontalAccel, -_player.MovementSpeed, _player.MovementSpeed);
        } else if (_player.Vel.X < 0) {
            _player.Vel.X = MathF.Min(_player.Vel.X + horizontalAccel, 0);
        } else if (_player.Vel.X > 0) {
            _player.Vel.X = MathF.Max(_player.Vel.X - horizontalAccel, 0);
        }

        _player.Move(_player.Vel);
    }
}
