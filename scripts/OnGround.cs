using Godot;
using System;

public partial class OnGround : PlayerState {
    public override void Enter() {

    }

    public override void Update(double delta) {
        if (Input.IsActionJustPressed("Jump")) {
            _stateMachine.ChangeState("Jump");
            return;
        }
    }
}
