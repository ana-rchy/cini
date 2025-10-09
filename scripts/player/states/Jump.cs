using System.Collections.Generic;
using Godot;

public partial class Jump : PlayerState {
    Vector2 _startingPos;
    float _gravity, _timeSinceJump;
    
    
    public override bool Prereqs() =>
        _inputBuffer.ShouldJump();

    public override void Enter(Dictionary<string, object> dict = null) {
        _player.Vel.Y = GetJumpVelocityWithDistance(
            _player.MovementSpeed,
            _player.JumpHeight,
            _player.JumpRiseDistance
        );
        
        _gravity = GetGravityWithDistance(
            _player.MovementSpeed,
            _player.JumpHeight,
            _player.JumpRiseDistance
        );
        
        _startingPos = _player.Pos.Vec;
        _timeSinceJump = 0;
        
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

        _timeSinceJump += delta;
    }
    
    public override void Update_StateTransitions(float _) {
        if (CommonInAirTransitions()) return;

        if (_player.Vel.Y >= 0) {
            _stateMachine.ChangeState<Falling>();
            return;
        }

        if (!Input.IsActionPressed("Jump")) {
            float remainingHeight = Mathf.Abs((_startingPos.Y - _player.JumpHeight) - _player.Pos.y);
            float totalJumpTime = _player.JumpRiseDistance / _player.MovementSpeed;
            float remainingTime = totalJumpTime - _timeSinceJump;
            
            _stateMachine.ChangeState<ShortJump>(dict: new() {
                { "RemainingHeight", remainingHeight },
                { "RemainingTime", remainingTime }
            });
            
            return;
        }
    }
}
