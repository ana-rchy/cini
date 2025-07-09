using System;
using Godot;

public partial class InputBuffer : Node {
    [ExportGroup("Buffer Lengths")]
    [Export] float _jumpBufferTime;
    
    [ExportGroup("node refs")]
    [Export] Timer _jumpBufferTimer;
    
    public bool ShouldJump { get; private set; }
    
    public override void _Ready() {
        _jumpBufferTimer.OneShot = true;
        _jumpBufferTimer.Timeout += _OnJumpBufferTimeout;
    }
    
    public override void _PhysicsProcess(double _) {
        if (Input.IsActionJustPressed("Jump")) {
            ShouldJump = true;
            _jumpBufferTimer.Start(_jumpBufferTime);
        }
    }
    
    
    void _OnJumpBufferTimeout() {
        ShouldJump = false;
    }
    
    
    public void StopJumpBufferTimer() {
        _jumpBufferTimer.Stop();
        ShouldJump = false;
    }
}
