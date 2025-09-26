using System;
using Godot;

public partial class WallBounce : PlayerState {
    Timer _pushoffTimer;
    float _gravity;
    
    
    public override void _Ready() {
        base._Ready();
        
        _pushoffTimer = new();
        _pushoffTimer.OneShot = true;
        AddChild(_pushoffTimer);
    }
    
    public override void Enter() {
        _player.Vel.X = _player.WallBouncePushoffSpeed * _player.GetWallNormal();
        
        _gravity = -_player.Vel.Y / _player.WallBounceTime;
        
        _player.TimeSinceJump = 0;

        _pushoffTimer.Start(_player.WallBouncePushoffTime);
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
}
