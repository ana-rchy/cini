using Godot;
using System;

public partial class Player : Node2D {
    [ExportGroup("node refs")]
    [Export] Collider _collider;
    
    Vector2PP _pos;
    
    float _speed = 1.5f;
    
    public override void _Ready() {
        _pos = new(this);
    }
    
    public override void _PhysicsProcess(double _) {
        if (Input.IsActionJustPressed("Left")) {
            MoveX(-_speed);
            GD.Print($"({_pos.x}, {_pos.y})");
        }
        if (Input.IsActionJustPressed("Right")) {
            MoveX(_speed);
            GD.Print($"({_pos.x}, {_pos.y})");
        }
        if (Input.IsActionJustPressed("Up")) {
            MoveY(-_speed);
            GD.Print($"({_pos.x}, {_pos.y})");
        }
        if (Input.IsActionJustPressed("Down")) {
            MoveY(_speed);
            GD.Print($"({_pos.x}, {_pos.y})");
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
    
    bool MoveX(float distance) {
        int dir = Mathf.Sign(distance);
        float remainder = distance - (int) distance;
        
        for (int i = 0; i < (int) Mathf.Abs(distance); i++) {
            if (TestTileCollision(dir, 0)) {
                return true;
            }
            
            _pos.x += dir;
        }
        
        _pos.x += remainder;
        return false;
    }
    
    bool MoveY(float distance) {
        int dir = Mathf.Sign(distance);
        float absDistance = Mathf.Abs(distance);
        
        float remainder = distance - (int) distance;
        
        GD.Print(remainder);
        
        for (int i = 0; i < (int) absDistance; i++) {
            if (TestTileCollision(0, dir)) {
                GD.Print("colliding");
                return true;
            }
            
            _pos.y += dir;
        }
        
        _pos.y += remainder;
        return false;
    }
    
    
    bool InTile() => TestTileCollision(0, 0);
    bool OnGround() => TestTileCollision(0, 1) && !InTile();
    bool OnCeiling() => TestTileCollision(0, -1) && !InTile();
    bool OnWall() => (TestTileCollision(1, 0) || TestTileCollision(-1, 0)) && !InTile();
}
