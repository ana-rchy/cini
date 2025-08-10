using System;
using Godot;

public class Vector2PP {
    Vector2 _floatPos;
    Vector2I _intPos;
    Node2D _associatedNode;

    public Vector2 v {
        get {
            return _floatPos;
        }
        set {
            _floatPos = value;
            _intPos = (Vector2I) value;
            _associatedNode.Position = (Vector2) _intPos;
        }
    }

    public float x {
        get {
            return _floatPos.X;
        }
        set {
            _floatPos.X = value;
            _intPos.X = (int) value;
            _associatedNode.Position = (Vector2) _intPos;
        }
    }
    public float y {
        get {
            return _floatPos.Y;
        }
        set {
            _floatPos.Y = value;
            _intPos.Y = (int) value;
            _associatedNode.Position = (Vector2) _intPos;
        }
    }

    public Vector2I vInt => _intPos;
    public int xInt => _intPos.X;
    public int yInt => _intPos.Y;

    public Vector2 Remainder => _floatPos - _intPos;


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
