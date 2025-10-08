using System;
using System.Collections.Generic;
using Godot;

public partial class Falling : PlayerState {
    float _gravity;
    
    
    public override bool EarlyExitStateTransitions() {
        if (_player.OnGround()) {
            _stateMachine.ChangeState<OnGround>();
            return true;
        }
        
        if (_player.OnWall() && Input.IsActionPressed("Grab")) {
            _stateMachine.ChangeState<Climbing>();
            return true;
        }
        
        if (Input.IsActionPressed("FastFall")) {
            _stateMachine.ChangeState<FastFall>();
            return true;
        }
        
        return false;
    }

    public override void Enter(Dictionary<string, object> dict = null) {
        _gravity = GetGravityWithDistance(
            _player.MovementSpeed,
            _player.JumpHeight,
            _player.JumpFallDistance
        );
    }
    

    public override void Update(float delta) {
        if (_player.Vel.Y < 0) {
            Clip(_player.Vel * delta, _player.DashClipThreshold);
        }
        
        // wall sliding
        float gravity = _gravity;
        float terminalVelocity = _player.TerminalVelocity;
        float xInput = Input.GetAxis("Left", "Right");

        if (_player.OnWall() && xInput == -_player.GetWallNormal() && _player.Vel.Y > 0) {
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
