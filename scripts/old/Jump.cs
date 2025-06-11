using Godot;
using System;

public partial class Jump : PlayerState {
    float _gravity;

    
    public override void Enter() {
        _player.Vel.Y = (2 * _player.JumpHeight * _player.MovementSpeed) / _player.JumpRiseDistance;
        
        _gravity = (-2 * _player.JumpHeight * MathF.Pow(_player.MovementSpeed, 2))
                   / MathF.Pow(_player.JumpRiseDistance, 2);
        
        _player.JumpTimeBeforeShortJump = 0;
    }

    public override void Update(float delta) {
        _player.Vel.X = _player.GetHorizontalVel(_player.FramesToMaxSpeedInAir, delta);
        
        Vector2 accelTerm = new(0, _player.GetVerletAccelTerm(_gravity, delta));
        _player.Vel = _player.Move(_player.Vel + accelTerm) - accelTerm;

        _player.Vel.Y += _gravity * delta;
        
        
        _player.JumpTimeBeforeShortJump += delta;
        

        _stateMachine.InAirCommonStateTransitions(_player);
        
        if (Input.IsActionPressed("FastFall")) {
            _stateMachine.ChangeState("FastFall");
            return;
        }

        if (_player.Vel.Y <= 0) {
            _stateMachine.ChangeState("Falling");
            return;
        }

        if (!Input.IsActionPressed("Jump")) {
            _stateMachine.ChangeState("ShortJump");
            return;
        }
    }
}
