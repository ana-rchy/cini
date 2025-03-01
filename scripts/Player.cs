using Godot;
using System;

public partial class Player : CharacterBody2D {
    [Export] public float MovementSpeed;
    [Export] public float FramesToMaxSpeed;
    [Export] public float FramesToMaxSpeedInAir;

    [Export] public float JumpMinHeight;
    [Export] public float JumpMaxHeight;
    [Export] public float JumpRiseDistance;
    [Export] public float JumpFallDistance;

    [Export] public float FastFallMultiplier;

    public Vector2 Velocity = Vector2.Zero;
}
