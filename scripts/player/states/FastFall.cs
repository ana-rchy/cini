using Godot;
using System;

public partial class FastFall : PlayerState {
    float _gravity;
    
    
    public override bool Prereqs() =>
        Input.IsActionPressed("FastFall");
    
    public override bool EarlyExitStateTransitions() {
        if (_player.OnGround()) {
            _stateMachine.ChangeState<OnGround>();
            return true;
        }
        
        return false;
    }

    public override void Enter() {
        _gravity = GetGravity(
                _player.MovementSpeed,
                _player.JumpHeight,
                _player.JumpFallDistance
        );
        
        _gravity *= _player.FastFallGravityMultiplier;
    }


    public override void Update(float delta) {
        HorizontalMovement(_player.MovementSpeed, _player.FramesToMaxSpeedInAir, delta);

        float yAccel = GetVerletAccelTerm(_gravity, delta);
        float yPosChange = MathF.Min(_player.Vel.Y * delta + yAccel, _player.FastFallTerminalVelocity * delta);

        _player.MoveY(yPosChange);
        _player.Vel.Y = MathF.Min(_player.Vel.Y + _gravity * delta, _player.FastFallTerminalVelocity);
    }
    
    public override void Update_StateTransitions() {
        if (CommonInAirTransitions()) return;

        if (!Input.IsActionPressed("FastFall")) {
            _stateMachine.ChangeState<Falling>();
            return;
        }
    }
}
