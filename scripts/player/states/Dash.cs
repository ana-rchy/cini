using System;
using Godot;

public partial class Dash : PlayerState {
    [Export] Sprite2D _playerSprite;
    [Export] Timer _postDashWallbounceLeniencyTimer;
    Timer _dashTimer;
    

    public override void _Ready() {
        base._Ready();
        
        string assertMsg(object obj) => $"'{GetParent().Name}/{Name}' is missing '{nameof(obj)}'";
        Logging.Assert(_playerSprite is Sprite2D, assertMsg(_playerSprite));
        Logging.Assert(_postDashWallbounceLeniencyTimer is Timer, assertMsg(_postDashWallbounceLeniencyTimer));
        
        _dashTimer = new();
        _dashTimer.OneShot = true;
        AddChild(_dashTimer);
    }
    
    public override bool Prereqs() =>
        Input.IsActionJustPressed("Dash") && _player.DashCharges > 0 &&
        Input.GetVector("Left", "Right", "Up", "Down") != Vector2.Zero;
    
    public override void Enter() {
        _player.Vel = Input.GetVector("Left", "Right", "Up", "Down") * _player.DashSpeed;

        _dashTimer.Start(_player.DashLength);

        _player.DashCharges--;
        
        _playerSprite.SelfModulate = new(1, 0, 1); // TODO: make a better coloring system for slimes
    }
    

    public override void Update(float delta) {
        if (_player.Vel.Normalized() != Vector2.Down) {
            Clip(_player.Vel * delta, _player.DashClipThreshold);
        }
        
        _player.MoveAndSlide(_player.Vel * delta);
    }
    
    public override void Update_StateTransitions(float _) {
        if (_player.Vel.Normalized() == Vector2.Up && _inputBuffer.ShouldJump()
            && _player.OnWall(_player.WallBounceDistanceLeniency)
        ) {
            _stateMachine.ChangeState<WallBounce>();
            return;
        }
        
        if (_player.OnGround() && _inputBuffer.ShouldJump()) {
            _stateMachine.ChangeState<Jump>();
            return;
        }
        
        if (_dashTimer.IsStopped()) {
            _stateMachine.ChangeState<Falling>();
            return;
        }
    }
    

    public override void Exit() {
        _playerSprite.SelfModulate = new(1, 1, 1);
        
        if (_player.Vel.Normalized() == Vector2.Up) {
            _postDashWallbounceLeniencyTimer.Start(_player.WallBounceTimeLeniencyInFrames / 60);
        }
    }
}
