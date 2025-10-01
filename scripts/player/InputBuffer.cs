using System;
using Godot;

public partial class InputBuffer : Node {
    [ExportGroup("node refs")]
    [Export] Player _player;
    
    Timer _jumpBufferTimer;
    bool _shouldJump;
    
    
    public override void _Ready() {
        _jumpBufferTimer = new();
        
        _jumpBufferTimer.OneShot = true;
        _jumpBufferTimer.Timeout += _OnJumpBufferTimeout;
        
        AddChild(_jumpBufferTimer);
    }
    
    public override void _PhysicsProcess(double _) {
        if (Input.IsActionJustPressed("Jump")) {
            _shouldJump = true;
            _jumpBufferTimer.Start(_player.JumpBufferTime);
        }
    }
    
    
    void _OnJumpBufferTimeout() {
        _shouldJump = false;
    }
    
    
    public bool ShouldJump() => Input.IsActionJustPressed("Jump") || _shouldJump;
    
    public void StopJumpBufferTimer() {
        _jumpBufferTimer.Stop();
        _shouldJump = false;
    }
}
