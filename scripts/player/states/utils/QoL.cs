using Godot;
using System;

namespace PlayerStateUtils {
public static class QoL {
    public static void WallClip(Player player) {
        if (!player.OnCeiling()) {
            return;
        }

        for (int x = 1; x <= player.WallClipThreshold; x++) {
            if (!player.CheckCollision(new(-x, 0), offset2: new(0, -1))) {
                player.MoveX(-x);
                break;
            }

            if (!player.CheckCollision(new(x, 0), offset2: new(0, -1))) {
                player.MoveX(x);
                break;
            }
        }
    }
}
}
