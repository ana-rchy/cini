using Godot;
using System;

public partial class ShortJump : PlayerState {
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
        
        // the principle is to convert the progress from a full jump into progress for a short jump
        //
        // progress in time and progress in distance are the same for the jump (assuming one direction is constantly held)
        //
        // so get the progress through the full jump as a float from 0-1, and lerp the velocity using that
        // (the velocity graph is a linear line so it works perfectly)
        float fullJumpProgress = (_player.TimeSinceJump * _player.MovementSpeed) / _player.JumpRiseDistance;
        
        _player.Vel.Y = Mathf.Lerp(_player.Vel.Y, 0, fullJumpProgress);
    }
    

    public override void Update(float delta) {
        HorizontalMovement(_player.MovementSpeed, _player.FramesToMaxSpeedInAir, delta);

        float yAccel = GetVerletAccelTerm(_gravity, delta);
        float yPosChange = _player.Vel.Y * delta + yAccel;

        _player.MoveY(yPosChange);
        _player.Vel.Y += _gravity * delta;

        WallClip();
    }
    
    public override void Update_StateTransitions() {
        if (CommonInAirTransitions()) return;

        if (Input.IsActionPressed("FastFall")) {
            _stateMachine.ChangeState<FastFall>();
            return;
        }

        if (_player.Vel.Y >= 0) {
            _stateMachine.ChangeState<Falling>();
            return;
        }
    }
}
