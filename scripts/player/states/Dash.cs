using System.Collections.Generic;
using Godot;

public partial class Dash : PlayerState {
    [ExportGroup("node refs")]
    [Export] Sprite2D _playerSprite;
    [Export] Timer _postDashWallbounceLeniencyTimer;
    
    Timer _dashTimer, _coyoteTimer;
    ShaderMaterial _spriteShader;
    bool _enteredOnGround;
    

    public override void _Ready() {
        base._Ready();
        
        string assertMsg(object obj) => $"'{GetParent().Name}/{Name}' is missing '{nameof(obj)}'";
        Logging.Assert(_playerSprite is Sprite2D, assertMsg(_playerSprite));
        Logging.Assert(_postDashWallbounceLeniencyTimer is Timer, assertMsg(_postDashWallbounceLeniencyTimer));
        
        _dashTimer = new();
        _dashTimer.OneShot = true;
        AddChild(_dashTimer);
        
        _coyoteTimer = new();
        _coyoteTimer.OneShot = true;
        AddChild(_coyoteTimer);
    }
    
    public override bool Prereqs() =>
        Input.IsActionJustPressed("Dash") && _player.DashCharges > 0 &&
        Input.GetVector("Left", "Right", "Up", "Down") != Vector2.Zero;
    
    public override void Enter(Dictionary<string, object> dict = null) {
        Vector2 inputVec = Input.GetVector("Left", "Right", "Up", "Down");
        Vector2 basicDashVel = inputVec * _player.DashSpeed;
        
        // makes chained wavedashes go nyoom
        if (_player.Vel.Abs().X > basicDashVel.Abs().X
            && Mathf.Sign(inputVec.X) == Mathf.Sign(_player.Vel.X) && inputVec.Y > 0 // down-diagonal dash in the direction of velocity
        ) {
            // dont affect x velocity; i.e. preserve x speed
            _player.Vel.Y = basicDashVel.Y;
        } else {
            _player.Vel = inputVec * _player.DashSpeed;
        }
        
        _enteredOnGround = _player.OnGround();
        _dashTimer.Start(_player.DashLengthInFrames / 60);
        _player.DashCharges--;
        
        _spriteShader = (ShaderMaterial) _playerSprite.Material;
        Color spriteColor = (Color) _spriteShader.GetShaderParameter("color");
        _spriteShader.SetShaderParameter("color", spriteColor.Inverted());
    }
    

    public override void Update(float delta) {
        if (_player.Vel.Normalized() != Vector2.Down) {
            Clip(_player.Vel * delta, _player.DashWallClipThreshold);
        }
        
        _player.MoveAndSlide(_player.Vel * delta);
    }
    
    public override void Update_StateTransitions(float _) {
        float xInput = Input.GetAxis("Left", "Right");
        
        if (_player.Vel.Y == 0 && _player.OnGround()) {
            _coyoteTimer.Start(_player.CoyoteTimeInFrames / 60);
        }
        
        
        // super
        if ((_player.OnGround() || !_coyoteTimer.IsStopped()) && _inputBuffer.ShouldJump()) {
            _player.DashCharges = _player.DashChargeTotal;
            _stateMachine.ChangeState<Jump>();
            
            return;
        }
        
        // bounce
        if (_player.OnGround() && _inputBuffer.ShouldJump()
            && _player.Vel.Normalized() == Vector2.Down && !_enteredOnGround
        ) {
            _stateMachine.ChangeState<Jump>(dict: new() {
                { "JumpHeight", _player.BounceHeight },
                { "JumpDistance", _player.BounceDistance }
            });
            
            return;
        }
        
        // wavedash/wavebounce
        if (_player.OnGround() && _inputBuffer.ShouldJump()
            && !_enteredOnGround
            && _player.Vel.X != 0 && _player.Vel.Y > 0 // down-diagonal dash
        ) {
            _player.DashCharges = _player.DashChargeTotal;
            _player.Vel.X *= _player.WavedashSpeedMultiplier;
            
            _stateMachine.ChangeState<Jump>(dict: new() {
                { "JumpHeight", _player.BounceHeight },
                { "JumpDistance", _player.BounceDistance }
            });
            
            return;
        }
        
         if (_player.OnWall(_player.WallBounceDistanceLeniency) && _inputBuffer.ShouldJump()
            && _player.Vel.Normalized() == Vector2.Up
        ) {
            _stateMachine.ChangeState<WallBounce>();
            return;
        }

        
        if (_player.OnWall() && xInput == -_player.GetWallNormal() && _player.Vel.Y < 0
            && _dashTimer.IsStopped()
        ) {
            _stateMachine.ChangeState<WallSlide>();
            return;
        }
        
        if (_dashTimer.IsStopped() && !_coyoteTimer.IsStopped()) {
            _stateMachine.ChangeState<OnGround>(dict: new() {
                { "RemainingCoyoteTime", _coyoteTimer.TimeLeft }     
            });
            
            return;
        }
        
        if (_dashTimer.IsStopped() && _coyoteTimer.IsStopped()) {
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
