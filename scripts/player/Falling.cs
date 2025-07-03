using Godot;
using System;

public partial class Falling : PlayerState {
    float _gravity;

    public override void Enter() {
        _gravity = (2 * _player.JumpHeight * MathF.Pow(_player.MovementSpeed, 2))
                   / MathF.Pow(_player.JumpFallDistance, 2);
    }

    public override void Update(float delta) {
        _player.Vel.X = _player.GetMovementXVel(_player.Vel.X, _player.FramesToMaxSpeedInAir);
        _player.MoveX(_player.Vel.X * delta);

        float accelTerm = _player.GetVerletAccelTerm(_gravity, delta);
        
        _player.MoveY(_player.Vel.Y * delta + accelTerm);
        _player.Vel.Y = MathF.Min(_player.Vel.Y + _gravity * delta, _player.TerminalVelocity);
        
        
        if (_player.OnGround()) {
            _stateMachine.ChangeState("OnGround");
            return;
        }
        
        if (_player.OnWall() && Input.IsActionJustPressed("Jump") && Input.GetAxis("Left", "Right") != 0) {
           _stateMachine.ChangeState("WallJump");
           return;
        }
        
        if (Input.IsActionPressed("FastFall")) {
            _stateMachine.ChangeState("FastFall");
            return;
        }
        
        // _stateMachine.InAirCommonStateTransitions(_player);
    }
}
