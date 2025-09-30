using Godot;
using System;

public partial class WallJump : PlayerState {
    Timer _pushoffTimer;
    float _gravity;


    public override void _Ready() {
        base._Ready();
        
        _pushoffTimer = new();
        _pushoffTimer.OneShot = true;
        AddChild(_pushoffTimer);
    }
    
    public override bool Prereqs() => 
        _inputBuffer.ShouldJump() && Input.GetAxis("Left", "Right") != 0
        && (
            _player.OnWall()
            || (_player.OnWall(_player.WallJumpLeniency) && _player.GetWallNormal(_player.WallJumpLeniency) != 0)
        );

    public override void Enter() {
        _player.Vel.X = _player.WallJumpPushoffSpeed * _player.GetWallNormal(_player.WallJumpLeniency);
        
        _player.Vel.Y = GetJumpVelocityWithTime(_player.WallJumpHeight, _player.WallJumpTime);
        _gravity = GetGravityWithTime(_player.WallJumpHeight, _player.WallJumpTime);

        _player.TimeSinceJump = 0;
        _pushoffTimer.Start(_player.WallJumpPushoffTime);
        
        // prevent jump being activated again in another state
        _inputBuffer.StopJumpBufferTimer();
    }
    

    public override void Update(float delta) {
        if (!_pushoffTimer.IsStopped()) {
            _player.MoveX(_player.Vel.X * delta);
        } else {
            BasicXMovement(_player.MovementSpeed, _player.FramesToMaxSpeedInAir, delta);
        }

        float yAccel = GetVerletAccelTerm(_gravity, delta);
        float yPosChange = _player.Vel.Y * delta + yAccel;
        
        Clip(0, yPosChange, _player.WallClipThreshold);

        _player.MoveY(yPosChange);
        _player.Vel.Y += _gravity * delta;

        _player.TimeSinceJump += delta;
    }
    
    public override void Update_StateTransitions(float _) {
        if (CommonInAirTransitions()) return;

        if (_player.Vel.Y >= 0) {
            _stateMachine.ChangeState<Falling>();
            return;
        }

        if (!Input.IsActionPressed("Jump")) {
            _stateMachine.ChangeState<ShortJump>();
            return;
        }
    }
}
