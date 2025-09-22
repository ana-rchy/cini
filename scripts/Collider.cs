using Godot;
using System;

[Tool]
public partial class Collider : Node2D {
    [Export] public int x, y;
    [Export] public int Width = 8;
    [Export] public int Height = 8;

    [Export] Color _colliderColor = new(0, 0, 1, 0.5f);
    
    public int Left {
        get { return (int) GlobalPosition.X + x; }
        set { Width -= value - Left; x += value - Left; }
    }
    public int Right {
        get { return Left + Width; }
        set { Width += value - Right; }
    }
    public int Top {
        get { return (int) GlobalPosition.Y + y; }
        set { Height -= value - Top; y += value - Top; }
    }
    public int Bottom {
        get { return Top + Height; }
        set { Height += value - Bottom; }
    }
    public Vector2I Centre => new((Left + Right) / 2, (Top + Bottom) / 2);

    
    // doesnt draw otherwise fsr
    public override void _Process(double delta) {
        QueueRedraw();
    }
    
    public override void _Draw() {
        if (Engine.IsEditorHint() || GetTree().DebugCollisionsHint) {
            DrawRect(new(x, y, Width, Height), _colliderColor);
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
