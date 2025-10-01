using System;
using Godot;

public partial class Dash : PlayerState {
    [ExportGroup("node refs")]
    [Export] Sprite2D _playerSprite;
    [Export] Timer _postDashWallbounceLeniencyTimer;
    
    Timer _dashTimer;
    ShaderMaterial _spriteShader;
    

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
        
        _spriteShader = (ShaderMaterial) _playerSprite.Material;
        Color spriteColor = (Color) _spriteShader.GetShaderParameter("color");
        _spriteShader.SetShaderParameter("color", spriteColor.Inverted());
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
        Color spriteColor = (Color) _spriteShader.GetShaderParameter("color");
        _spriteShader.SetShaderParameter("color", spriteColor.Inverted());
        
        if (_player.Vel.Normalized() == Vector2.Up) {
            _postDashWallbounceLeniencyTimer.Start(_player.WallBounceTimeLeniencyInFrames / 60);
        }
    }
}
