using Godot;
using System;

public partial class ClimbShortJump : PlayerState {
    float _gravity;
    

    public override bool Prereqs() =>
        !Input.IsActionPressed("Jump");
    
    public override void Enter() {
        _player.Vel.Y = GetJumpVelocity(
                _player.MovementSpeed,
                _player.ShortJumpHeight,
                _player.ShortJumpRiseDistance
        );
        
        _gravity = GetGravity(
                _player.MovementSpeed,
                _player.ShortJumpHeight,
                _player.ShortJumpRiseDistance
        );
        
        // reference same line in ShortJump for explanation
        float fullJumpProgress = (_player.TimeSinceJump * _player.MovementSpeed) / _player.JumpRiseDistance;
        
        _player.Vel.Y = Mathf.Lerp(_player.Vel.Y, 0, fullJumpProgress);
    }
    

    public override void Update(float delta) {
        HorizontalMovement(_player.MovementSpeed, _player.FramesToMaxSpeed, delta);

        float yAccel = GetVerletAccelTerm(_gravity, delta);
        float yPosChange = _player.Vel.Y * delta + yAccel;

        _player.MoveY(yPosChange);
        _player.Vel.Y += _gravity * delta;
    }
    
    public override void Update_StateTransitions() {
        if (_player.Vel.Y >= 0) {
            if (_player.OnWall() && Input.IsActionPressed("Grab")) {
                _stateMachine.ChangeState<Climbing>();
            } else {
                _stateMachine.ChangeState<Falling>();
            }
            
            return;
        }
        
        if (!_player.OnWall() || _player.Stamina <= 0) {
            _stateMachine.ChangeState<Falling>();
            return;
        }
    }
}
