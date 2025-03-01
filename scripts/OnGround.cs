using Godot;
using System;

public partial class OnGround : PlayerState {
    [Export] RayCast2D _onGroundRayCast;

    public override void Enter() {
        _player.Velocity.Y = 0;
    }

    public override void Update(float delta) {
        if (!_onGroundRayCast.IsColliding()) {
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
            _player.Velocity.X = Mathf.Clamp(_player.Velocity.X + dir * horizontalAccel, -_player.MovementSpeed, _player.MovementSpeed);
        } else if (_player.Velocity.X < 0) {
            _player.Velocity.X = MathF.Min(_player.Velocity.X + horizontalAccel, 0);
        } else if (_player.Velocity.X > 0) {
            _player.Velocity.X = MathF.Max(_player.Velocity.X - horizontalAccel, 0);
        }

        _player.MoveAndCollide(new Vector2(_player.Velocity.X, 0) * delta);
    }
}
