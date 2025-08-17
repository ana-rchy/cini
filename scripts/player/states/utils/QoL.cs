using Godot;
using System;

namespace PlayerStateUtils {
public static class QoL {
    // TODO: rewrite
    public static void WallClip(Player player) {
        if (!player.OnCeiling()) {
            return;
        }

        for (int x = 1; x <= player.WallClipThreshold; x++) {
            if (!player.TestXThenYCollision(-x, -1)) {
                player.MoveX(-x);
                break;
            }

            if (!player.TestXThenYCollision(x, -1)) {
                player.MoveX(x);
                break;
            }
        }
    }
}
}
