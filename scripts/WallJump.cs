using Godot;
using System;

public partial class WallJump : PlayerState {
    [Export] Timer _pushoffTimer;
    
    float _gravity;


    public override void _Ready() {
        base._Ready();

        _gravity = (-2 * _player.WallJumpHeight * MathF.Pow(_player.MovementSpeed, 2))
                   / MathF.Pow(_player.WallJumpRiseDistance, 2);
    }


    public override void Enter() {
        _player.Vel.Y = (2 * _player.WallJumpHeight * _player.MovementSpeed) / _player.WallJumpRiseDistance;
        _player.JumpTimeBeforeShortJump = 0;
        
        _player.Vel.X = _player.WallJumpPushoffSpeed * _player.GetWallNormal().X;
        _pushoffTimer.Start();
    }

    public override void Update(float delta) {
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
    
    void _OnPushoffTimeout() {
        _stateMachine.ChangeState("Falling");
    }
}
