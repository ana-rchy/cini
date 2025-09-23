using Godot;
using System;

[Tool]
public partial class Collider : Node2D {
    [Export] public Vector2I Size = new(8, 8);
    [Export] Color _colliderColor = new(0, 0, 1, 0.5f);
    
    public Vector2PP Pos;
    
    public int Width {
        get { return Size.X; }
        set { Size = new(value, Size.Y); }
    }
    public int Height {
        get { return Size.Y; }
        set { Size = new(Size.X, value); }
    }
    
    public int Left => (int) GlobalPosition.X;
    public int Right => Left + Width;
    public int Top => (int) GlobalPosition.Y;
    public int Bottom => Top + Height;
    public Vector2I Centre => new((Left + Right) / 2, (Top + Bottom) / 2);
    

    
    public override void _Ready() {
        Pos = new(this, Position);
    }
    
    // doesnt draw otherwise fsr
    public override void _Process(double delta) {
        QueueRedraw();
    }
    
    public override void _Draw() {
        if (Engine.IsEditorHint() || GetTree().DebugCollisionsHint) {
            DrawRect(new(0, 0, Size.X, Size.Y), _colliderColor);
        }
    }


    public bool OnTopOf(Collider other) => Intersects(other, 0, 1);
    public bool OnBottomOf(Collider other) => Intersects(other, 0, -1);
    public bool OnLeftOf(Collider other) => Intersects(other, 1, 0);
    public bool OnRightOf(Collider other) => Intersects(other, -1, 0);
    
    public bool Intersects(Collider other, int offsetX = 0, int offsetY = 0) {
        return
            (Right + offsetX) > other.Left &&
            (Left + offsetX) < other.Right &&
            (Bottom + offsetY) > other.Top &&
            (Top + offsetY) < other.Bottom;
    }
}
