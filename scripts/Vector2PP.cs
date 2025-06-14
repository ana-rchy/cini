using System;
using Godot;

public class Vector2PP {
    Vector2 _floatPos;
    Vector2I _intPos;
    
    public float x {
        get {
            return _floatPos.X;
        }
        set {
            _floatPos.X = value;
            _intPos.X = (int) MathF.Round(value, MidpointRounding.ToPositiveInfinity);
        }
    }
    public float y {
        get {
            return _floatPos.Y;
        }
        set {
            _floatPos.Y = value;
            _intPos.Y = (int) MathF.Round(value, MidpointRounding.ToPositiveInfinity);
        }
    }
    
    public int xInt => _intPos.X;
    public int yInt => _intPos.Y;
    
    
    public Vector2PP() {
        x = y = 0;
    }
    
    public Vector2PP(float value) {
        x = y = value;
    }
    
    public Vector2PP(float x, float y) {
        this.x = x;
        this.y = y;
    }
}
