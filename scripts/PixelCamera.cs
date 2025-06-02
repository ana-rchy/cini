using Godot;
using System;

public partial class PixelCamera : Camera2D {
    public Vector2 RealPosition;
    
    public override void _Process(double _) {
        GlobalPosition = RealPosition.Round();
    }
}
