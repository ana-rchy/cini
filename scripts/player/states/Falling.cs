using Godot;
using System;

public partial class Falling : PlayerState {
    float _gravity;

    public override void Enter() {
        _gravity = (2 * _player.JumpHeight * MathF.Pow(_player.MovementSpeed, 2))
                   / MathF.Pow(_player.JumpFallDistance, 2);
    }

    public override void Update(float delta) {
        // wall sliding
        float gravity = _gravity;
        float terminalVelocity = _player.TerminalVelocity;
        
        if ((Input.IsActionPressed("Left") && _player.OnLeftWall()) ||
            (Input.IsActionPressed("Right") && _player.OnRightWall()))
        {
            gravity = _gravity / _player.WallSlidingDivisor;
            terminalVelocity = _player.TerminalVelocity / _player.WallSlidingDivisor;
        }
        
        HorizontalMovement(_player.FramesToMaxSpeedInAir, delta);

        float accelTerm = GetVerletAccelTerm(gravity, delta);
        float yPosChange = MathF.Min(_player.Vel.Y * delta + accelTerm, terminalVelocity * delta);
        
        _player.MoveY(yPosChange);
        _player.Vel.Y = MathF.Min(_player.Vel.Y + gravity * delta, terminalVelocity);
        
        
        CommonInAirTransitions();
        
        if (Input.IsActionPressed("FastFall")) {
            _stateMachine.ChangeState("FastFall");
            return;
        }
    }
}
