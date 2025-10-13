using System.Collections.Generic;
using Godot;

public partial class Jump : PlayerState {
    Vector2 _startingPos;
    float _gravity, _timeSinceJump;
    
    float _jumpHeight, _jumpDistance;
    float _jumpTime => _jumpDistance / _player.MovementSpeed;
    
    
    public override bool Prereqs() =>
        _inputBuffer.ShouldJump();

    public override void Enter(Dictionary<string, object> dict = null) {
        _jumpHeight = _player.JumpHeight;
        _jumpDistance = _player.JumpDistance;
        if (dict != null) {
            _jumpHeight = (float) dict.GetValueOrDefault("JumpHeight", _jumpHeight);
            _jumpDistance = (float) dict.GetValueOrDefault("JumpDistance", _jumpDistance);
        }
        
        _player.Vel.Y = GetJumpVelocityWithDistance(
            _jumpHeight,
            _jumpDistance,
            _player.MovementSpeed
        );
        
        _gravity = GetGravityWithDistance(
            _jumpHeight,
            _jumpDistance,
            _player.MovementSpeed
        );
        
        _startingPos = _player.Pos.Vec;
        _timeSinceJump = 0;
        
        // prevent jump being activated again in another state
        _inputBuffer.StopJumpBufferTimer();
    }
    

    public override void Update(float delta) {
        float yAccel = GetVerletAccelTerm(_gravity, delta);
        float yPosChange = _player.Vel.Y * delta + yAccel;
        
        Clip(0, yPosChange, _player.JumpWallClipThreshold);
        
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
            float remainingTime = _jumpTime - _timeSinceJump;
            
            _stateMachine.ChangeState<ShortJump>(dict: new() {
                { "RemainingHeight", remainingHeight },
                { "RemainingTime", remainingTime }
            });
            
            return;
        }
    }
}
