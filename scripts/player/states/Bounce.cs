using System;
using Godot;

public partial class Bounce : PlayerState {
    float _gravity;
    
    
    public override bool Prereqs() =>
        _inputBuffer.ShouldJump() && _player.OnGround();

    public override void Enter() {
        _player.Vel.Y = GetJumpVelocityWithDistance(
            _player.MovementSpeed,
            _player.BounceHeight,
            _player.BounceRiseDistance
        );
        
        _gravity = GetGravityWithDistance(
            _player.MovementSpeed,
            _player.BounceHeight,
            _player.BounceRiseDistance
        );
        
        _player.TimeSinceJump = 0;
        _player.DashCharges = _player.DashChargeTotal;

        // prevent jump being activated again in another state
        _inputBuffer.StopJumpBufferTimer();
    }
    
    
    public override void Update(float delta) {
        float yAccel = GetVerletAccelTerm(_gravity, delta);
        float yPosChange = _player.Vel.Y * delta + yAccel;
        
        Clip(0, yPosChange, _player.WallClipThreshold);
        
        BasicXMovement(_player.MovementSpeed, _player.FramesToMaxSpeedInAir, delta);

        _player.MoveY(yPosChange);
        _player.Vel.Y += _gravity * delta;

        _player.TimeSinceJump += delta;
    }
    
    public override void Update_StateTransitions(float _) {
        if (CommonInAirTransitions()) return;

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
