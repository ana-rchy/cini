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
            _pos.x -= _speed;
            GD.Print($"({_pos.x}, {_pos.y})");
        }
        if (Input.IsActionJustPressed("Right")) {
            _pos.x += _speed;
            GD.Print($"({_pos.x}, {_pos.y})");
        }
        if (Input.IsActionJustPressed("Up")) {
            _pos.y -= _speed;
            GD.Print($"({_pos.x}, {_pos.y})");
        }
        if (Input.IsActionJustPressed("Down")) {
            _pos.y += _speed;
            GD.Print($"({_pos.x}, {_pos.y})");
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
    
    bool MoveX(float distance) {
        int dir = Mathf.Sign(distance);
        float remainder = distance - (int) distance;
        
        bool collided = false;
        
        for (int i = 0; i < (int) Mathf.Abs(distance); i++) {
            if (TestTileCollision(dir, 0)) {
                collided = true;
                break;
            }
            
            _pos.x += 1;
        }
        
        _pos.x += remainder;
        return collided;
    }
    
    
    bool InTile() => TestTileCollision(0, 0);
    bool OnGround() => TestTileCollision(0, 1) && !InTile();
    bool OnCeiling() => TestTileCollision(0, -1) && !InTile();
    bool OnWall() => (TestTileCollision(1, 0) || TestTileCollision(-1, 0)) && !InTile();
}
