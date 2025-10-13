using System;
using Godot;

public partial class InputBuffer : Node {
    [ExportGroup("node refs")]
    [Export] Player _player;
    
    Timer _jumpBufferTimer, _dashBufferTimer;
    bool _shouldJump, _shouldDash;
    
    
    public override void _Ready() {
        _jumpBufferTimer = new();
        _jumpBufferTimer.OneShot = true;
        _jumpBufferTimer.Timeout += _OnJumpBufferTimeout;
        AddChild(_jumpBufferTimer);
        
        _dashBufferTimer = new();
        _dashBufferTimer.OneShot = true;
        _dashBufferTimer.Timeout += _OnDashBufferTimeout;
        AddChild(_dashBufferTimer);
    }
    
    public override void _PhysicsProcess(double _) {
        if (Input.IsActionJustPressed("Jump")) {
            _shouldJump = true;
            _jumpBufferTimer.Start(_player.JumpBufferTime);
        }
        
        if (Input.IsActionJustPressed("Dash")) {
            _shouldDash = true;
            _dashBufferTimer.Start(_player.DashBufferTime);
        }
    }
    
    
    void _OnJumpBufferTimeout() {
        _shouldJump = false;
    }
    
    void _OnDashBufferTimeout() {
        _shouldDash = false;
    }
    
    
    public bool ShouldJump() => Input.IsActionJustPressed("Jump") || _shouldJump;
    
    public void StopJumpBufferTimer() {
        _jumpBufferTimer.Stop();
        _shouldJump = false;
    }
    
    public bool ShouldDash() => Input.IsActionJustPressed("Dash") || _shouldDash;
    
    public void StopDashBufferTimer() {
        _dashBufferTimer.Stop();
        _shouldDash = false;
    }
}
