using Godot;
using System;

public partial class Player : CharacterBody2D {
    [Export] public float MovementSpeed;
    [Export] public float FramesToMaxSpeed;
    [Export] public float FramesToMaxSpeedInAir;

    [Export] public float JumpHeight;
    [Export] public float ShortJumpHeight;
    [Export] public float JumpRiseDistance;
    [Export] public float ShortJumpRiseDistance;
    [Export] public float JumpFallDistance;

    [Export] public float FastFallMultiplier;

    // cause Velocity is already taken
    public Vector2 Vel = Vector2.Zero;

    public float JumpTimeBeforeShortJump = 0;

    public void Move(Vector2 velocity) {
        Velocity = new(velocity.X, -velocity.Y);
        MoveAndSlide();
    }
}
