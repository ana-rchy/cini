using Godot;
using System;

public partial class FastFall : PlayerState {
    [Export] float _terminalVelocity;
    [Export] float _gravity;
    [Export] RayCast2D _onGroundRayCast;

    public override void Enter() {

    }

    public override void Update(double delta) {
        if (_onGroundRayCast.IsColliding()) {
            _stateMachine.ChangeState("OnGround");
            return;
        }

        if (!Input.IsActionPressed("FastFall")) {
            _stateMachine.ChangeState("Falling");
            return;
        }
    }
}
