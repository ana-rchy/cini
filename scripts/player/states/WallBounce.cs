using System.Collections.Generic;
using Godot;

public partial class WallBounce : PlayerState {
    [ExportGroup("node refs")]
    [Export] Timer _postDashWallbounceLeniencyTimer;
    
    Timer _pushoffTimer;
    Vector2 _startingPos, _startingVel;
    float _gravity, _timeSinceJump;
    
    
    public override void Init() {
        string assertMsg(object obj) => $"'{GetParent().Name}/{Name}' is missing '{nameof(obj)}'";
        Logging.Assert(_postDashWallbounceLeniencyTimer is Timer, assertMsg(_postDashWallbounceLeniencyTimer));
        
        _pushoffTimer = new();
        _pushoffTimer.OneShot = true;
        AddChild(_pushoffTimer);
        
        _postDashWallbounceLeniencyTimer.OneShot = true;
    }
    
    
    public override bool Prereqs() =>
        _inputBuffer.ShouldJump()
        && _player.Vel.Abs().Y > _player.WallbounceMinimumSpeed
        && (
            _player.OnWall()
            || ( // close enough to *one* wall, but *not* close-enough-to-and-between *two* walls
                _player.OnWall(_player.WallBounceDistanceLeniency)
                && _player.GetWallNormal(_player.WallBounceDistanceLeniency) != 0
            )
        );
    
    public override void Enter(Dictionary<string, object> dict = null) {
        _player.Vel.X = _player.WallBounceMovementSpeed * _player.GetWallNormal(_player.WallBounceDistanceLeniency);
        
        _gravity = Mathf.Abs(_player.Vel.Y / _player.WallBounceTime);
        
        _pushoffTimer.Start(_player.WallBouncePushoffTime);
        _startingPos = _player.Pos.Vec;
        _startingVel = _player.Vel;
        _timeSinceJump = 0;
        
        // prevent jump being activated again in another state
        _inputBuffer.StopJumpBufferTimer();
    }
    
    
    public override void Update(float delta) {
        if (!_pushoffTimer.IsStopped()) {
            _player.MoveX(_player.Vel.X * delta);
        } else {
            BasicXMovement(_player.WallBounceMovementSpeed, _player.FramesToMaxSpeedInAir, delta);
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
        
        if (!Input.IsActionPressed("Jump")) {
            // kinematic displacement formula
            float maxHeight = _startingVel.Y * _player.WallBounceTime;
            maxHeight += 0.5f * _gravity * Mathf.Pow(_player.WallBounceTime, 2);
            //
            maxHeight = Mathf.Abs(maxHeight);
            
            float maxDistance = _player.WallBounceMovementSpeed * _player.WallBounceTime;
            
            float remainingHeight = Mathf.Abs((_startingPos.Y - maxHeight) - _player.Pos.y);
            float remainingDistance = Mathf.Abs(maxDistance - (_player.WallBounceMovementSpeed * _timeSinceJump));
            
            _stateMachine.ChangeState<ShortJump>(dict: new() {
                { "RemainingHeight", remainingHeight },
                { "RemainingDistance", remainingDistance },
                { "MovementSpeed", _player.WallBounceMovementSpeed }
            });
            
            return;
        }

        if (_player.Vel.Y >= 0) {
            _stateMachine.ChangeState<Falling>();
            return;
        }
    }
    
    
    public override void _PhysicsProcess(double _) {
        if (_stateMachine.CurrentState is WallBounce) {
            return;
        }
        
        if (_inputBuffer.ShouldJump() && _player.OnWall(_player.WallBounceDistanceLeniency)
            && !_postDashWallbounceLeniencyTimer.IsStopped()
            && _player.Vel.Abs().Y > _player.WallbounceMinimumSpeed
        ) {
            _stateMachine.ChangeState<WallBounce>();              
        }
    }
}
