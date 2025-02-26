using Godot;
using System;

public partial class Player : CharacterBody2D {
    [Export] public Curve JumpArc;
    [Export] public float JumpHeight;
    [Export] public float JumpTime;

    public Vector2 VelocityPerSec;
}
