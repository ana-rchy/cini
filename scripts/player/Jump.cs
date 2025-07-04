using Godot;
using System;

public partial class Jump : PlayerState {
    float _gravity;

    
    public override void Enter() {
        _player.Vel.Y = (-2 * _player.JumpHeight * _player.MovementSpeed) / _player.JumpRiseDistance;
        
        _gravity = (2 * _player.JumpHeight * MathF.Pow(_player.MovementSpeed, 2))
                   / MathF.Pow(_player.JumpRiseDistance, 2);
        
        _player.TimeSinceJump = 0;
    }

    public override void Update(float delta) {
        _player.Vel.X = _player.GetMovementXVel(_player.Vel.X, _player.FramesToMaxSpeedInAir, delta);
        _player.MoveX(_player.Vel.X * delta);
       
        float accelTerm = _player.GetVerletAccelTerm(_gravity, delta);
        
        _player.MoveY(_player.Vel.Y * delta + accelTerm);
        _player.Vel.Y += _gravity * delta;
        
        CheckWallClip();
        
        _player.TimeSinceJump += delta;
        
        
        CommonInAirTransitions();
        
        if (Input.IsActionPressed("FastFall")) {
            _stateMachine.ChangeState("FastFall");
            return;
        }

        if (_player.Vel.Y >= 0) {
            _stateMachine.ChangeState("Falling");
            return;
        }

        if (!Input.IsActionPressed("Jump")) {
            _stateMachine.ChangeState("ShortJump");
            return;
        }
    }
}
