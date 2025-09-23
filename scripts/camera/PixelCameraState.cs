using System;
using Godot;

public partial class PixelCameraState : State {
    protected PixelCamera _camera;
    
    
    public override void _Ready() {
        base._Ready();
        
        Node grandparent = GetNode("../..");
        Logging.Assert(grandparent is PixelCamera, $"{grandparent.Name} is not a '{typeof(PixelCamera)}'");
        _camera = (PixelCamera) grandparent;
    }
}
