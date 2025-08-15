using Godot;
using System;

[Tool]
public partial class Collider : Node2D {
    [Export] int _x, _y;
    [Export] int _width = 8;
    [Export] int _height = 8;

    [Export] Color _colliderColor = new(0, 0, 1, 0.5f);

    public int Left => (int) GlobalPosition.X + _x;
    public int Right => Left + _width;
    public int Top => (int) GlobalPosition.Y + _y;
    public int Bottom => Top + _height;
    public Vector2I Centre => new((Left + Right) / 2, (Top + Bottom) / 2);


    public override void _Draw() {
        if (Engine.IsEditorHint() || GetTree().DebugCollisionsHint) {
            DrawRect(new(_x, _y, _width, _height), _colliderColor);
        }
    }

    public override void _Process(double delta) {
        QueueRedraw();
    }


    public bool Intersects(Collider other, int offsetX = 0, int offsetY = 0) {
        return
            (Right + offsetX) > other.Left &&
            (Left + offsetX) < other.Right &&
            (Bottom + offsetY) > other.Top &&
            (Top + offsetY) < other.Bottom;
    }
}
