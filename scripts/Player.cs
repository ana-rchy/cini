using Godot;
using System;

public partial class Player : Node {
    [Export] public Curve JumpArc;
    [Export] public float JumpHeight;

    public Vector2 Velocity;
}
