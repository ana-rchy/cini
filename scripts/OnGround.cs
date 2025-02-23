using Godot;
using System;

public partial class OnGround : State {
    public override void Enter() {

    }

    public override void Update() {
        if (Input.IsActionJustPressed("Jump")) {
            _stateMachine.ChangeState("InAir");
            return;
        }
    }
}
