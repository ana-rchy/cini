using Godot;
using System;

public partial class FastFall : PlayerState {
    float _gravity;

    
    public override void Enter() {
        _gravity = (2 * _player.JumpHeight * MathF.Pow(_player.MovementSpeed, 2))
                   / MathF.Pow(_player.JumpFallDistance, 2);
        
        _gravity *= _player.FastFallGravityMultiplier;
    }


    public override void Update(float delta) {
        _player.Vel.X = _player.GetMovementXVel(_player.Vel.X, _player.FramesToMaxSpeedInAir, delta);
        _player.MoveX(_player.Vel.X * delta);
        
        float accelTerm = _player.GetVerletAccelTerm(_gravity, delta);
        float yPosChange = MathF.Min(_player.Vel.Y * delta + accelTerm, _player.FastFallTerminalVelocity * delta);
        
        _player.MoveY(yPosChange);
        _player.Vel.Y = MathF.Min(_player.Vel.Y + _gravity * delta, _player.FastFallTerminalVelocity);
        
        
        if (_player.OnGround()) {
            _stateMachine.ChangeState("OnGround");
            return;
        }
        
        if (_player.OnWall() && Input.IsActionJustPressed("Jump") && Input.GetAxis("Left", "Right") != 0) {
           _stateMachine.ChangeState("WallJump");
           return;
        } 
        
        if (_player.OnWall() && Input.IsActionJustPressed("Jump") && Input.GetAxis("Left", "Right") == 0) {
            _stateMachine.ChangeState("NeutralWallJump");
            return;
        }
        
        if (!Input.IsActionPressed("FastFall")) {
            _stateMachine.ChangeState("Falling");
            return;
        }
    }
}
