using Godot;
using System;

public partial class Falling : PlayerState {
    float _gravity;
    
    
    public override bool EarlyExitStateTransitions() {
        if (_player.OnGround()) {
            _stateMachine.ChangeState<OnGround>();
            return true;
        }
        
        if (Input.IsActionPressed("FastFall")) {
            _stateMachine.ChangeState<FastFall>();
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
    }
    

    public override void Update(float delta) {
        // wall sliding
        float gravity = _gravity;
        float terminalVelocity = _player.TerminalVelocity;

        if ((Input.IsActionPressed("Left") && _player.OnLeftWall()) ||
            (Input.IsActionPressed("Right") && _player.OnRightWall())
        ) {
            // TODO: make the gravity wall up-sliding thingy into a proper feature
            gravity = _gravity / _player.WallSlidingDivisor;
            terminalVelocity = _player.TerminalVelocity / _player.WallSlidingDivisor;
        }

        BasicXMovement(_player.MovementSpeed, _player.FramesToMaxSpeedInAir, delta);

        float yAccel = GetVerletAccelTerm(gravity, delta);
        float yPosChange = MathF.Min(_player.Vel.Y * delta + yAccel, terminalVelocity * delta);

        _player.MoveY(yPosChange);
        _player.Vel.Y = MathF.Min(_player.Vel.Y + gravity * delta, terminalVelocity);
    }
    
    public override void Update_StateTransitions(float _) {
        if (CommonInAirTransitions()) return;

        if (Input.IsActionPressed("FastFall")) {
            _stateMachine.ChangeState<FastFall>();
            return;
        }
    }
}
