using System;
using Godot;

public partial class InputBuffer : Node {
    [ExportGroup("Buffer Lengths")]
    [Export] float _jumpBufferTime;
    
    [ExportGroup("node refs")]
    [Export] Timer _jumpBufferTimer;
    
    bool _shouldJump;
    
    
    public override void _Ready() {
        Logging.Assert(_jumpBufferTimer is Timer, $"'{GetParent().Name}/{Name}' is missing '{nameof(_jumpBufferTimer)}'");
        
        _jumpBufferTimer.OneShot = true;
        _jumpBufferTimer.Timeout += _OnJumpBufferTimeout;
    }
    
    public override void _PhysicsProcess(double _) {
        if (Input.IsActionJustPressed("Jump")) {
            _shouldJump = true;
            _jumpBufferTimer.Start(_jumpBufferTime);
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
