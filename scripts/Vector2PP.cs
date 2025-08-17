using System;
using Godot;

public class Vector2PP {
    Vector2 _floatPos;
    Vector2I _intPos; // truncated from _floatPos
    Node2D _associatedNode; // used for forcing a Node2D's Position to be the same as _intPos
    

    public Vector2 Vec { // vector
        get => _floatPos;
        set {
            _floatPos = value;
            _intPos = (Vector2I) value;
            _associatedNode.Position = (Vector2) _intPos;
        }
    }

    public float x {
        get => _floatPos.X;
        set {
            _floatPos.X = value;
            _intPos.X = (int) value;
            _associatedNode.Position = (Vector2) _intPos;
        }
    }
    public float y {
        get => _floatPos.Y;
        set {
            _floatPos.Y = value;
            _intPos.Y = (int) value;
            _associatedNode.Position = (Vector2) _intPos;
        }
    }

    public Vector2I VecInt => _intPos;
    public int xInt => _intPos.X;
    public int yInt => _intPos.Y;

    public Vector2 VecRemainder => _floatPos - _intPos;
    public float xRemainder => x - xInt;
    public float yRemainder => y - yInt;


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
    
    public Vector2PP(Node2D associatedNode, Vector2 vec) {
        _associatedNode = associatedNode;
        Vec = vec;
    }
}
