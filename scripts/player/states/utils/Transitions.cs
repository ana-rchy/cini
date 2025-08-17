using Godot;
using System;

namespace PlayerStateUtils {
public static class Transitions {
    // state transitions that happen whenever youre in the air
    public static bool CommonInAirTransitions(Player player, StateMachine stateMachine, InputBuffer inputBuffer) {
        if (player.OnGround()) {
            stateMachine.ChangeState<OnGround>();
            return true;
        }

        if (player.OnWall() && inputBuffer.ShouldJump() && Input.GetAxis("Left", "Right") != 0) {
            stateMachine.ChangeState<WallJump>();
            return true;
        }

        if (player.OnWall() && inputBuffer.ShouldJump() && Input.GetAxis("Left", "Right") == 0) {
            stateMachine.ChangeState<NeutralWallJump>();
            return true;
        }

        if (Input.IsActionJustPressed("Dash")) {
            stateMachine.ChangeState<Dash>();
            return true;
        }

        if (player.OnWall() && Input.IsActionPressed("Grab")) {
            stateMachine.ChangeState<Climbing>();
            return true;
        }

        if (player.OnCeiling() && Input.IsActionPressed("Grab")) {
            stateMachine.ChangeState<Hanging>();
            return true;
        }

        return false;
    }
} 
}
