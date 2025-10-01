using System;
using Godot;

public partial class WallBounce : PlayerState {
    [Export] Timer _postDashWallbounceLeniencyTimer;
    Timer _pushoffTimer;
    float _gravity;
    
    
    public override void _Ready() {
        base._Ready();
        
        string assertMsg(object obj) => $"'{GetParent().Name}/{Name}' is missing '{nameof(obj)}'";
        Logging.Assert(_postDashWallbounceLeniencyTimer is Timer, assertMsg(_postDashWallbounceLeniencyTimer));
        
        _pushoffTimer = new();
        _pushoffTimer.OneShot = true;
        AddChild(_pushoffTimer);
        
        _postDashWallbounceLeniencyTimer.OneShot = true;
    }
    
    public override bool Prereqs() =>
        _inputBuffer.ShouldJump()
        && (
            _player.OnWall()
            || (_player.OnWall(_player.WallBounceDistanceLeniency) && _player.GetWallNormal(_player.WallBounceDistanceLeniency) != 0)
        );
    
    public override void Enter() {
        _player.Vel.X = _player.WallBouncePushoffSpeed * _player.GetWallNormal(_player.WallBounceDistanceLeniency);
        
        _gravity = -_player.Vel.Y / _player.WallBounceTime;
        
        _player.TimeSinceJump = 0;
        _pushoffTimer.Start(_player.WallBouncePushoffTime);
        
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
    }
    
    
    public override void _PhysicsProcess(double _) {
        if (_stateMachine.CurrentState is WallBounce) {
            return;
        }
        
        if (_inputBuffer.ShouldJump() && _player.OnWall(_player.WallBounceDistanceLeniency)
            && !_postDashWallbounceLeniencyTimer.IsStopped()
        ) {
            _stateMachine.ChangeState<WallBounce>();              
        }
    }
}
