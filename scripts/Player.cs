using Godot;
using System;

public partial class Player : CharacterBody2D {
    // TODO: add timer timeouts to here too
    [Export] public float MovementSpeed;
    [Export] public float FramesToMaxSpeed;
    [Export] public float FramesToMaxSpeedInAir;

    [Export] public float JumpHeight;
    [Export] public float JumpRiseDistance;
    [Export] public float JumpFallDistance;
    
    [Export] public float ShortJumpHeight;
    [Export] public float ShortJumpRiseDistance;
    
    [Export] public float WallJumpHeight;
    [Export] public float WallJumpRiseDistance;
    [Export] public float WallJumpPushoffSpeed;
    
    [Export] public float NeutralWallJumpHeight;
    [Export] public float NeutralWallJumpRiseDistance;
    [Export] public float NeutralWallJumpPushoffSpeed;

    [Export] public float FastFallMultiplier;

    // cause Velocity is already taken
    // and Velocity doesnt let you access its properties (Velocity.X/Y)
    public Vector2 Vel = Vector2.Zero;

    public float JumpTimeBeforeShortJump = 0;

    // base.Velocity is Y-down, vel in this function is Y-up
    // all player physics are done Y-up
    // 
    // returns post-MoveAndSlide() velocity, which adjusts for things like collision
    // (e.g. hitting a wall sets base.Velocity.X to 0)
    public Vector2 Move(Vector2 vel) {
        base.Velocity = new(vel.X, -vel.Y);
        
        MoveAndSlide();
        
        return new(base.Velocity.X, -base.Velocity.Y);
    }
    
    public float GetHorizontalVel(float framesToMaxSpeed, float delta) {
        // 1 / delta = how many timesteps
        // speed / (1 / d) = convert from position per sec to position per timestep
        // the rest is just, accel = speed / time
        float horizontalAccel = (MovementSpeed / (1 / delta)) / (framesToMaxSpeed / 60);
        float dir = Input.GetAxis("Left", "Right");

        // TODO: implement a soft cap instead of hard cap on speed
        if (dir != 0 && Mathf.Abs(Vel.X) <= MovementSpeed) {
            return Mathf.Clamp(Vel.X + dir * horizontalAccel, -MovementSpeed, MovementSpeed);
        } else if (Vel.X < 0) {
            return MathF.Min(Vel.X + horizontalAccel, 0);
        } else if (Vel.X > 0) {
            return MathF.Max(Vel.X - horizontalAccel, 0);
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
