using Godot;
using System;

public partial class InAir : PlayerState {
    [Export] RayCast2D _onGroundRayCast;

    public override void Enter() {

    }

    public override void Update() {
        if (_onGroundRayCast.IsColliding()) {
            _stateMachine.ChangeState("OnGround");
            return;
        }

        if (Input.IsActionPressed("FastFall")) {
            _stateMachine.ChangeState("FastFall");
            return;
        }
    }
}
