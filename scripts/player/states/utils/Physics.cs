using Godot;
using System;

namespace PlayerStateUtils {
public static class Physics {
    // 'basic x movement' = the movement you do with only left/right keys
    public static bool BasicXMovement(float movementSpeed, float framesToMaxSpeed, float delta, Player player) {
        player.Vel.X = GetBasicXMovementVel(movementSpeed, framesToMaxSpeed, player.Vel.X, player.ReelbackLerpStrength, delta);
        
        return player.MoveX(player.Vel.X * delta);
    }
    
    static float GetBasicXMovementVel(
        float movementSpeed,
        float framesToMaxSpeed,
        float xVel,
        Curve reelbackLerpStrength,
        float delta
    ) {
        // accel = speed / time
        float speed = Mathf.Abs(xVel);
        float xAccel = movementSpeed / (framesToMaxSpeed / 60);
        float inputDir = Input.GetAxis("Left", "Right");
        
        float newXVel = xVel + xAccel * inputDir * delta;
        
        if (inputDir == 0) {
            if (xVel < 0) {
                newXVel = Mathf.Min(newXVel + xAccel * delta, 0);
            } else if (xVel > 0) {
                newXVel = Mathf.Max(newXVel - xAccel * delta, 0);
            }
        }
        
        if (speed <= movementSpeed && inputDir != 0) {
            newXVel = Mathf.Clamp(newXVel, -movementSpeed, movementSpeed);
        }
        
        if (speed > movementSpeed) {
            int xVelDir = Mathf.Sign(xVel);
            
            if (inputDir == xVelDir) {
                newXVel = xVel;
            }
            
            // do reelback thingy
            newXVel = Mathf.Lerp(newXVel, 0, reelbackLerpStrength.Sample(Mathf.Abs(newXVel)) * delta);
        }
        
        return newXVel;
    }
    
    
    
    public static float GetJumpVelocityWithTime(float height, float time) {
        return (-2 * height) / time;
    }
    
    public static float GetGravityWithTime(float height, float time) {
       return (2 * height) / (time * time); 
    }
    
    public static float GetVerletAccelTerm(float accel, float deltaTime) {
        // velocity verlet formula:
        // pos += vel * dt + (0.5 * accel * dt^2) (same as kinematic displacement equation)
        // vel += acc * dt
        return 0.5f * accel * deltaTime * deltaTime;
    }
}
}
