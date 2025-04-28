using Godot;
using System;

public partial class Falling : PlayerState {
    float _gravity, _terminalVelocity;


    public override void _Ready() {
        base._Ready();

        _gravity = (-2 * _player.JumpHeight * MathF.Pow(_player.MovementSpeed, 2)) / MathF.Pow(_player.JumpFallDistance, 2);

        _terminalVelocity = -(2 * _player.JumpHeight * _player.MovementSpeed) / _player.JumpFallDistance;
    }


    public override void Update(float delta) {
        _player.Vel.X = _player.GetHorizontalVel(_player.FramesToMaxSpeedInAir, delta);

        float accelTerm = _player.GetVerletAccelTerm(_gravity, delta);
        _player.Translate(_player.Vel + new Vector2(0, accelTerm));

        _player.Vel.Y = MathF.Max(_player.Vel.Y + _gravity * delta, _terminalVelocity);
        
        
        if (_player.IsOnFloor()) {
            _stateMachine.ChangeState("OnGround");
            return;
        }

        if (Input.IsActionPressed("FastFall")) {
            _stateMachine.ChangeState("FastFall");
            return;
        }
    }
}
