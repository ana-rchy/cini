using Godot;
using System;

public partial class Falling : PlayerState {
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

        if (Input.IsActionPressed("FastFall")) {
            _stateMachine.ChangeState("FastFall");
            return;
        }

        _player.VelocityPerSec = new(
            _player.VelocityPerSec.X,
            MathF.Min(_player.VelocityPerSec.Y + _gravity, _terminalVelocity)
        );

        _player.MoveAndCollide(_player.VelocityPerSec * (float) delta);
    }
}
