using Godot;
using System;

public partial class WallJump : PlayerState {
    [Export] Timer _pushoffTimer;
    float _gravity;


    public override void _Ready() {
        base._Ready();
        
        Logging.Assert(_pushoffTimer is Timer, $"'{GetParent().Name}/{Name}' is missing '{nameof(_pushoffTimer)}'");
    }
    
    public override bool Prereqs() =>
        _player.OnWall() && _inputBuffer.ShouldJump() && Input.GetAxis("Left", "Right") != 0;

    public override void Enter() {
        _player.Vel.X = _player.WallJumpPushoffSpeed * _player.GetWallNormal();
        
        _player.Vel.Y = GetJumpVelocity(
                _player.MovementSpeed,
                _player.WallJumpHeight,
                _player.WallJumpRiseDistance
        );
        
        _gravity = GetGravity(
                _player.MovementSpeed,
                _player.WallJumpHeight,
                _player.WallJumpRiseDistance
        );

        _player.TimeSinceJump = 0;

        _pushoffTimer.OneShot = true;
        _pushoffTimer.Start(_player.WallJumpPushoffTime);
    }
    

    public override void Update(float delta) {
        _player.MoveX(_player.Vel.X * delta);

        float yAccel = GetVerletAccelTerm(_gravity, delta);
        float yPosChange = _player.Vel.Y * delta + yAccel;

        _player.MoveY(yPosChange);
        _player.Vel.Y += _gravity * delta;

        WallClip();

        _player.TimeSinceJump += delta;
    }
    
    public override void Update_StateTransitions() {
        if (CommonInAirTransitions()) return;

        if (_pushoffTimer.IsStopped()) {
            _stateMachine.ChangeState<Falling>();
            return;
        }

        if (!Input.IsActionPressed("Jump")) {
            _stateMachine.ChangeState<ShortJump>();
            return;
        }
    }
}
