using Godot;
using System;

public partial class InputOverlayKey : ColorRect {
    public override void _PhysicsProcess(double _) {
        if (Input.IsActionPressed(Name)) {
            Color = new("#66ff96");
        } else {
            Color = new("#ffffff");
        }
    }
}
