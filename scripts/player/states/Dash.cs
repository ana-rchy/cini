using System;
using Godot;

public partial class Dash : PlayerState {
    [Export] Timer _dashTimer;
    [Export] Sprite2D _playerSprite;

    public override bool Prereqs() {
        if (_player.DashCharges > 0 && Input.GetVector("Left", "Right", "Up", "Down") != Vector2.Zero) {
            return true;
        }

        return false;
    }

    public override void Enter() {
        _player.Vel = Input.GetVector("Left", "Right", "Up", "Down") * _player.DashSpeed;

        _dashTimer.WaitTime = _player.DashLength;
        _dashTimer.OneShot = true;
        _dashTimer.Start();

        _player.DashCharges--;
        _playerSprite.SelfModulate = new(1, 0, 1);
    }

    public override void Update(float delta) {
        _player.MoveAndSlide(_player.Vel * delta);


        if (_dashTimer.IsStopped()) {
            _stateMachine.ChangeState("Falling");
            return;
        }
    }

    public override void Exit() {
        _playerSprite.SelfModulate = new(1, 1, 1);
    }



    void DashClip() {
        if (_player.InAir()) {
            return;
        }
        
        // TODO: this
    }
}
