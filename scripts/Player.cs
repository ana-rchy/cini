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

    public void Translate(Vector2 velocity) {
        Velocity = new(velocity.X, -velocity.Y);
        MoveAndSlide();
    }
    
    public float GetHorizontalVel(float framesToMaxSpeed, float delta) {
        // 1 / delta = how many timesteps
        // speed / (1 / d) = convert from position per sec to position per timestep
        // the rest is just, accel = speed / time
        float horizontalAccel = (MovementSpeed / (1 / delta)) / (framesToMaxSpeed / 60);
        float dir = Input.GetAxis("Left", "Right");

        if (dir != 0) {
            return Vel.X = Mathf.Clamp(Vel.X + dir * horizontalAccel, -MovementSpeed, MovementSpeed);
        } else if (Vel.X < 0) {
            return Vel.X = MathF.Min(Vel.X + horizontalAccel, 0);
        } else if (Vel.X > 0) {
            return Vel.X = MathF.Max(Vel.X - horizontalAccel, 0);
        }
        
        return 0;
    }
    
    
    public float GetVerletAccelTerm(float gravity, float delta) {
        // velocity verlet equation:
        // pos += vel * dt + (0.5 * accel * dt^2)
        // 
        // tho here its just dt (delta), because move_and_slide also applies delta
        return 0.5f * gravity * delta;
    }
}
