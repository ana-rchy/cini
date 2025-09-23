using Godot;
using System;

public partial class Room : CollisionShape2D {
    public Collider Collider { get; private set; }
    
    public static int MinWidth { get; private set; }
    public static int MinHeight { get; private set; }
    
    public override void _Ready() {
        Logging.Assert(Shape is RectangleShape2D, $"Room {Name}'s {nameof(Shape)} is not {typeof(RectangleShape2D)}");
        
        RectangleShape2D rectShape = (RectangleShape2D) Shape;
        
        
        const int TILE_SIZE = 8;
        Vector2 viewportSize = GetViewport().GetVisibleRect().Size;
        
        MinWidth = TILE_SIZE * Mathf.CeilToInt(viewportSize.X / TILE_SIZE);
        MinHeight = TILE_SIZE * Mathf.CeilToInt(viewportSize.Y / TILE_SIZE);
        
        Collider = new();
        AddChild(Collider);
        
        Collider.Pos.Vec = (Position - (rectShape.Size / 2));
        Collider.Size = (Vector2I) rectShape.Size;
        
        // needed so that it doesnt mess up the Collider's Left/Right/Top/Bottom calculations
        // (cause it uses GlobalPosition, which means parent nodes, like this one, affect that)
        Position = Vector2.Zero;
    }
}
