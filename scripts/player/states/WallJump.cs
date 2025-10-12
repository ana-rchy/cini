using System.Collections.Generic;
using Godot;

public partial class WallJump : PlayerState {
    Timer _pushoffTimer;
    Vector2 _startingPos;
    float _gravity, _timeSinceJump;


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
            || (_player.OnWall(_player.WallJumpDistanceLeniency) && _player.GetWallNormal(_player.WallJumpDistanceLeniency) != 0)
        );

    public override void Enter(Dictionary<string, object> dict = null) {
        _player.Vel.X = _player.WallJumpMovementSpeed * _player.GetWallNormal(_player.WallJumpDistanceLeniency);
        
        _player.Vel.Y = GetJumpVelocityWithDistance(
            _player.WallJumpHeight,
            _player.WallJumpDistance,
            _player.WallJumpMovementSpeed
        );
        
        _gravity = GetGravityWithDistance(
            _player.WallJumpHeight,
            _player.WallJumpDistance,
            _player.WallJumpMovementSpeed
        );
        
        _pushoffTimer.Start(_player.WallJumpPushoffTime);
        _startingPos = _player.Pos.Vec;
        _timeSinceJump = 0;
        
        // prevent jump being activated again in another state
        _inputBuffer.StopJumpBufferTimer();
    }
    

    public override void Update(float delta) {
        if (!_pushoffTimer.IsStopped()) {
            _player.MoveX(_player.Vel.X * delta);
        } else {
            BasicXMovement(_player.WallJumpMovementSpeed, _player.FramesToMaxSpeedInAir, delta);
        }

        float yAccel = GetVerletAccelTerm(_gravity, delta);
        float yPosChange = _player.Vel.Y * delta + yAccel;
        
        Clip(0, yPosChange, _player.JumpWallClipThreshold);

        _player.MoveY(yPosChange);
        _player.Vel.Y += _gravity * delta;

        _timeSinceJump += delta;
    }
    
    public override void Update_StateTransitions(float _) {
        if (CommonInAirTransitions()) return;

        if (_player.Vel.Y >= 0) {
            _stateMachine.ChangeState<Falling>();
            return;
        }

        if (!Input.IsActionPressed("Jump")) {
            float remainingHeight = Mathf.Abs((_startingPos.Y - _player.WallJumpHeight) - _player.Pos.y);
            float remainingTime = _player.WallJumpTime - _timeSinceJump;
            
            _stateMachine.ChangeState<ShortJump>(dict: new() {
                { "RemainingHeight", remainingHeight },
                { "RemainingTime", remainingTime }
            });
            
            return;
        }
    }
}
