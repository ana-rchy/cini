using System;
using Godot;

public partial class Climbing : PlayerState {
    public override bool Prereqs() {
        if (_player.Stamina > 0 && _player.OnWall()) {
            return true;
        }

        return false;
    }

    public override void Enter() {
        _player.Vel = Vector2.Zero;
    }

    public override void Update(float delta) {
        _player.Vel.Y = Input.GetAxis("Up", "Down") * _player.ClimbingSpeed;
        _player.MoveY(_player.Vel.Y * delta);


        if (_player.Vel.Y == 0) {
            _player.Stamina -= _player.GrabbingStaminaCost * delta;
        } else {
            _player.Stamina -= _player.ClimbingStaminaCost * delta;
        }


        if (Input.IsActionPressed("Grab") && _inputBuffer.ShouldJump) {
            _stateMachine.ChangeState("WallJump");
        }

        if (!Input.IsActionPressed("Grab") && _inputBuffer.ShouldJump) {
            if (Input.GetAxis("Left", "Right") != 0) {
                _stateMachine.ChangeState("WallJump");
                return;
            } else {
                _stateMachine.ChangeState("NeutralWallJump");
                return;
            }
        }

        if (!Input.IsActionPressed("Grab") || !_player.OnWall() || _player.Stamina <= 0) {
            _stateMachine.ChangeState("Falling");
            return;
        }
    }
}
