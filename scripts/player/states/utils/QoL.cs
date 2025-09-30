using System.Collections.Generic;
using Godot;

namespace PlayerStateUtils {
public static class QoL {
    // moving the player a bit if theyre about to collide into smth
    public static void Clip(Vector2 posChange, int clipThreshold, Player player) {
        bool willCollide = player.TestMove(posChange);
        bool isDiagonal = posChange.X != 0 && posChange.Y != 0;
        
        if (!willCollide || isDiagonal) {
            return;
        }
        
        for (int i = 1; i <= clipThreshold; i++) {
            if (posChange.X != 0) {
                if (!player.TestMove(new List<Vector2>() { new(0, -i), posChange })) {
                    player.MoveY(-i);
                    return;
                }
                
                if (!player.TestMove(new List<Vector2>() { new(0, i), posChange })) {
                    player.MoveY(i);
                    return;
                }
            }
            
            if (posChange.Y != 0) {
                if (!player.TestMove(new List<Vector2>() { new(-i, 0), posChange })) {
                    player.MoveX(-i);
                    return;
                }
                
                if (!player.TestMove(new List<Vector2>() { new(i, 0), posChange })) {
                    player.MoveX(i);
                    return;
                }
            }
        }
    }
}
}
