using Godot;
using System;

public partial class Player : Node2D {
    [ExportGroup("node refs")]
    [Export] Collider _collider;
    
    int _posX { get { return (int) Position.X; } set { Position = new(value, Position.Y); }}
    int _posY { get { return (int) Position.Y; } set { Position = new(Position.X, value); }}
    
    public override void _Process(double _) {
        if (Input.IsActionJustPressed("Left")) {
            _posX -= 1;
        } else if (Input.IsActionJustPressed("Right")) {
            _posX += 1;
        }
        if (Input.IsActionJustPressed("Up")) {
            _posY -= 1;
        } else if (Input.IsActionJustPressed("Down")) {
            _posY += 1;
        }
        
        if (InTile()) {
            GD.Print("InTile");
        }
        if (OnGround()) {
            GD.Print("OnGround");
        }
        if (OnCeiling()) {
            GD.Print("OnCeiling");
        }
        if (OnWall()) {
            GD.Print("OnWall");
        }
    }
    
    bool TestTileCollision(int offsetX, int offsetY) {
        foreach (Collider tile in GetTree().GetNodesInGroup("Tiles")) {
            if (_collider.Intersects(tile, offsetX, offsetY)) {
                return true;
            }
        }
        
        return false;
    }
    
    
    bool InTile() => TestTileCollision(0, 0);
    bool OnGround() => TestTileCollision(0, 1) && !InTile();
    bool OnCeiling() => TestTileCollision(0, -1) && !InTile();
    bool OnWall() => (TestTileCollision(1, 0) || TestTileCollision(-1, 0)) && !InTile();
}
