using Godot;
using System;

public partial class Jump : PlayerState {
    float _gravity;


    public override void _Ready() {
        base._Ready();

        _gravity = (-2 * _player.JumpHeight * MathF.Pow(_player.MovementSpeed, 2)) / MathF.Pow(_player.JumpRiseDistance, 2);
    }


    public override void Enter() {
        _player.Vel.Y = (2 * _player.JumpHeight * _player.MovementSpeed) / _player.JumpRiseDistance;
        _player.JumpTimeBeforeShortJump = 0;
    }

    public override void Update(float delta) {
        _player.Vel.X = _player.GetHorizontalVel(_player.FramesToMaxSpeedInAir, delta);

        float accelTerm = _player.GetVerletAccelTerm(_gravity, delta);
        _player.Translate(_player.Vel + new Vector2(0, accelTerm));

        _player.Vel.Y += _gravity * delta;
        
        
        _player.JumpTimeBeforeShortJump += delta;
        

        _stateMachine.InAirCommonStateTransitions(_player);
        
        if (Input.IsActionPressed("FastFall")) {
            _stateMachine.ChangeState("FastFall");
            return;
        }

        if (_player.Vel.Y <= 0) {
            _stateMachine.ChangeState("Falling");
            return;
        }

        if (!Input.IsActionPressed("Jump")) {
            _stateMachine.ChangeState("ShortJump");
            return;
        }
    }
}
