using System;
using Godot;

public partial class Dash : PlayerState {
    [Export] Timer _dashTimer;
    [Export] Sprite2D _playerSprite;
    

    public override void _Ready() {
        base._Ready();
        
        string assertMsg(object obj) => $"'{GetParent().Name}/{Name}' is missing '{nameof(obj)}'";
        
        Logging.Assert(_dashTimer is Timer, assertMsg(_dashTimer));
        Logging.Assert(_playerSprite is Sprite2D, assertMsg(_playerSprite));
    }
    
    public override bool Prereqs() =>
        Input.IsActionJustPressed("Dash") && _player.DashCharges > 0 &&
        Input.GetVector("Left", "Right", "Up", "Down") != Vector2.Zero;
    
    public override void Enter() {
        _player.Vel = Input.GetVector("Left", "Right", "Up", "Down") * _player.DashSpeed;

        _dashTimer.WaitTime = _player.DashLength;
        _dashTimer.OneShot = true;
        _dashTimer.Start();

        _player.DashCharges--;
        
        _playerSprite.SelfModulate = new(1, 0, 1); // TODO: make a better coloring system for slimes
    }
    

    public override void Update(float delta) {
        _player.MoveAndSlide(_player.Vel * delta);
    }
    
    public override void Update_StateTransitions(float _) {
        if (_dashTimer.IsStopped()) {
            _stateMachine.ChangeState<Falling>();
            return;
        }
    }
    

    public override void Exit() {
        _playerSprite.SelfModulate = new(1, 1, 1);
    }



    void DashClip() {
        // TODO: this
    }
}
