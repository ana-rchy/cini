using Godot;
using System;

namespace PlayerStateUtils {
public static class Physics {
    public static bool HorizontalMovement(float movementSpeed, float framesToMaxSpeed, float delta, Player player) {
        player.Vel.X = GetMovementXVel(movementSpeed, framesToMaxSpeed, player.Vel.X, delta);
        
        return player.MoveX(player.Vel.X * delta);
    }
    
    static float GetMovementXVel(float movementSpeed, float framesToMaxSpeed, float playerXVel, float delta) {
        // accel = speed / time
        float horizontalAccel = movementSpeed / (framesToMaxSpeed / 60);
        float dir = Input.GetAxis("Left", "Right");

        // TODO: implement a soft cap instead of hard cap on speed
        if (dir != 0 && Mathf.Abs(playerXVel) <= movementSpeed) {
            return Mathf.Clamp(playerXVel + dir * horizontalAccel * delta, -movementSpeed, movementSpeed);
        } else if (playerXVel < 0) {
            return MathF.Min(playerXVel + horizontalAccel * delta, 0);
        } else if (playerXVel > 0) {
            return MathF.Max(playerXVel - horizontalAccel * delta, 0);
        }

        return 0;
    }
    
    
    
    public static float GetJumpVelocity(float movementSpeed, float height, float distance) {
        float velocity = -2 * height * movementSpeed;
        velocity /= distance;
        
        return velocity;
    }
    
    public static float GetGravity(float movementSpeed, float height, float distance) {
        float gravity = 2 * height * Mathf.Pow(movementSpeed, 2);
        gravity /= MathF.Pow(distance, 2);
        
        return gravity;
    }
    
    public static float GetVerletAccelTerm(float accel, float delta) {
        // velocity verlet equation:
        // pos += vel * dt + (0.5 * accel * dt^2)
        // vel += acc * dt
        return 0.5f * accel * delta * delta;
    }
}
}
