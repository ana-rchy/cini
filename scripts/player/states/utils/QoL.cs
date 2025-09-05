using Godot;
using System;

namespace PlayerStateUtils {
public static class QoL {
    // moving the player a bit if theyre about to collide into smth
    public static void Clip(Vector2 posChange, int clipThreshold, Player player) {
        bool willCollide = player.CheckCollision(posChange);
        bool isDiagonal = posChange.X != 0 && posChange.Y != 0;
        
        if (!willCollide || isDiagonal) {
            return;
        }
        
        Vector2 ogPos = player.Pos.Vec;
        
        for (int i = 1; i <= clipThreshold; i++) {
            player.Pos.Vec = ogPos;
            
            if (posChange.X != 0) {
                if (!player.MoveY(-i) && !player.CheckCollision(posChange)) {
                    return;
                }
                
                player.Pos.Vec = ogPos;
                
                if (!player.MoveY(i) && !player.CheckCollision(posChange)) {
                    return;
                }
            }
            
            if (posChange.Y != 0) {
                if (!player.MoveX(-i) && !player.CheckCollision(posChange)) {
                    return;
                }
                
                player.Pos.Vec = ogPos;
                
                if (!player.MoveX(i) && !player.CheckCollision(posChange)) {
                    return;
                }
            }
        }
        
        player.Pos.Vec = ogPos;
    }
}
}
