using System;
using Godot;

public partial class Hanging : PlayerState {
    public override void Enter() {
        _player.Vel = Vector2.Zero;
    }

    public override void Update(float delta) {
        _player.Vel.X = Input.GetAxis("Left", "Right") * _player.HangingMoveSpeed;
        _player.MoveX(_player.Vel.X * delta);


        if (_player.Vel.Y == 0) {
            _player.Stamina -= _player.HangingStaminaCost * delta;
        } else {
            _player.Stamina -= _player.HangingMoveStaminaCost * delta;
        }


        if (!Input.IsActionPressed("Grab") || !_player.OnCeiling() || _player.Stamina <= 0) {
            _stateMachine.ChangeState("Falling");
            return;
        }

        if (Input.IsActionJustPressed("Jump")) {
            _player.MoveY(1);
            _stateMachine.ChangeState("Falling");
            return;
        }
    }
}
