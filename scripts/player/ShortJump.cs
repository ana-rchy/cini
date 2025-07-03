using Godot;
using System;

public partial class ShortJump : PlayerState {
    float _gravity;

    
    public override void Enter() {
        _player.Vel.Y = (-2 * _player.ShortJumpHeight * _player.MovementSpeed) / _player.ShortJumpRiseDistance;
        
        _gravity = (2 * _player.ShortJumpHeight * MathF.Pow(_player.MovementSpeed, 2))
                   / MathF.Pow(_player.ShortJumpRiseDistance, 2);

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
        _player.Vel.X = _player.GetMovementXVel(_player.Vel.X, _player.FramesToMaxSpeedInAir, delta);
        _player.MoveX(_player.Vel.X * delta);
        
        float accelTerm = _player.GetVerletAccelTerm(_gravity, delta);
        
        _player.MoveY(_player.Vel.Y * delta + accelTerm);
        _player.Vel.Y += _gravity * delta;
        

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
        
        if (Input.IsActionPressed("FastFall")) {
            _stateMachine.ChangeState("FastFall");
            return;
        }

        if (_player.Vel.Y >= 0) {
            _stateMachine.ChangeState("Falling");
            return;
        }
    }
}
