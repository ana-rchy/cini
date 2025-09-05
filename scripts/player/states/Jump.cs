using Godot;
using System;
using QoL = PlayerStateUtils.QoL;

public partial class Jump : PlayerState {
    float _gravity;
    
    
    public override bool Prereqs() =>
        _inputBuffer.ShouldJump();

    public override void Enter() {
        _player.Vel.Y = GetJumpVelocity(
            _player.MovementSpeed,
            _player.JumpHeight,
            _player.JumpRiseDistance
        );
        
        _gravity = GetGravity(
            _player.MovementSpeed,
            _player.JumpHeight,
            _player.JumpRiseDistance
        );

        _player.TimeSinceJump = 0;
        
        // prevent jump being activated again in another state
        _inputBuffer.StopJumpBufferTimer();
    }
    

    public override void Update(float delta) {
        BasicXMovement(_player.MovementSpeed, _player.FramesToMaxSpeedInAir, delta);

        float yAccel = GetVerletAccelTerm(_gravity, delta);
        float yPosChange = _player.Vel.Y * delta + yAccel;
        
        Clip(0, yPosChange, _player.WallClipThreshold);

        _player.MoveY(yPosChange);
        _player.Vel.Y += _gravity * delta;

        _player.TimeSinceJump += delta;
    }
    
    public override void Update_StateTransitions(float _) {
        if (CommonInAirTransitions()) return;

        if (Input.IsActionPressed("FastFall")) {
            _stateMachine.ChangeState<FastFall>();
            return;
        }

        if (_player.Vel.Y >= 0) {
            _stateMachine.ChangeState<Falling>();
            return;
        }

        if (!Input.IsActionPressed("Jump")) {
            _stateMachine.ChangeState<ShortJump>();
            return;
        }
    }
}
