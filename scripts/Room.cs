using Godot;
using System;

public partial class Room : CollisionShape2D {
    [Export] public Collider Collider { get; private set; }
    
    public static int MinWidth { get; private set; }
    public static int MinHeight { get; private set; }
    
    public override void _Ready() {
        Logging.Assert(Collider is Collider, $"Room {Name}'s {nameof(Collider)} reference is missing");
        Logging.Assert(Shape is RectangleShape2D, $"Room {Name}'s {nameof(Shape)} is not {typeof(RectangleShape2D)}");
        
        
        const int TILE_SIZE = 8;
        Vector2 viewportSize = GetViewport().GetVisibleRect().Size;
        
        MinWidth = TILE_SIZE * Mathf.CeilToInt(viewportSize.X / TILE_SIZE);
        MinHeight = TILE_SIZE * Mathf.CeilToInt(viewportSize.Y / TILE_SIZE);
        
        
        RectangleShape2D rectShape = (RectangleShape2D) Shape;
        
        Collider.Left = (int) (Position.X - (rectShape.Size.X / 2));
        Collider.Right = Collider.Left + (int) rectShape.Size.X;
        Collider.Top = (int) (Position.Y - (rectShape.Size.Y / 2));
        Collider.Bottom = Collider.Top + (int) rectShape.Size.Y;
    }
}
