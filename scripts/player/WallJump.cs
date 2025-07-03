using Godot;
using System;

public partial class WallJump : PlayerState {
    [Export] Timer _pushoffTimer;
    float _gravity;

    
    public override void Enter() {
        _player.Vel.Y = (-2 * _player.WallJumpHeight * _player.MovementSpeed) / _player.WallJumpRiseDistance;
        
        _gravity = (2 * _player.WallJumpHeight * MathF.Pow(_player.MovementSpeed, 2))
                   / MathF.Pow(_player.WallJumpRiseDistance, 2);
        
        _player.Vel.X = _player.WallJumpPushoffSpeed * _player.GetWallNormal();
        
        _player.TimeSinceJump = 0;
        
        _pushoffTimer.OneShot = true;
        _pushoffTimer.Start(_player.WallJumpPushoffTime);
    }

    public override void Update(float delta) {
        _player.MoveX(_player.Vel.X * delta);
        
        float accelTerm = _player.GetVerletAccelTerm(_gravity, delta);
        
        _player.MoveY(_player.Vel.Y * delta + accelTerm);
        _player.Vel.Y += _gravity * delta;
        
        
        _player.TimeSinceJump += delta;
        

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
        
        if (_pushoffTimer.IsStopped()) {
            _stateMachine.ChangeState("Falling");
            return;
        }

        if (!Input.IsActionPressed("Jump")) {
            _stateMachine.ChangeState("ShortJump");
            return;
        }
    }
}
