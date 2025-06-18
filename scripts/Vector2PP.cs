using System;
using Godot;

public class Vector2PP {
    Vector2 _floatPos;
    Vector2I _intPos;
    Node2D _associatedNode;
    
    public float x {
        get {
            return _floatPos.X;
        }
        set {
            _floatPos.X = value;
            _intPos.X = (int) MathF.Round(value, MidpointRounding.ToPositiveInfinity);
            _associatedNode.Position = new(_intPos.X, _associatedNode.Position.Y);
        }
    }
    public float y {
        get {
            return _floatPos.Y;
        }
        set {
            _floatPos.Y = value;
            _intPos.Y = (int) MathF.Round(value, MidpointRounding.ToPositiveInfinity);
            _associatedNode.Position = new(_associatedNode.Position.X, _intPos.Y);
        }
    }
    
    public int xInt => _intPos.X;
    public int yInt => _intPos.Y;
    
    
    public Vector2PP(Node2D associatedNode) {
        _associatedNode = associatedNode;
        x = y = 0;
    }
    
    public Vector2PP(Node2D associatedNode, float value) {
        _associatedNode = associatedNode;
        x = y = value;
    }
    
    public Vector2PP(Node2D associatedNode, float x, float y) {
        _associatedNode = associatedNode;
        this.x = x;
        this.y = y;
    }
}
