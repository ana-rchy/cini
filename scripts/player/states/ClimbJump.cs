using System.Collections.Generic;
using Godot;

public partial class ClimbJump : PlayerState {
    Vector2 _startingPos;
    float _gravity, _timeSinceJump;


    public override bool Prereqs() =>
        _player.OnWall() && _inputBuffer.ShouldJump() && _player.Stamina >= 0;

    public override void Enter(Dictionary<string, object> dict = null) {
        _player.Vel.X = 0;

        _player.Vel.Y = GetJumpVelocityWithTime(_player.WallJumpHeight, _player.WallJumpTime);
        _gravity = GetGravityWithTime(_player.WallJumpHeight, _player.WallJumpTime);

        _startingPos = _player.Pos.Vec;
        _timeSinceJump = 0;
        
        _player.Stamina -= _player.ClimbJumpStaminaCost;

        // prevent jump being activated again in another state
        _inputBuffer.StopJumpBufferTimer();
    }


    public override void Update(float delta) {
        BasicXMovement(_player.MovementSpeed, _player.FramesToMaxSpeed, delta);

        float yAccel = GetVerletAccelTerm(_gravity, delta);
        float yPosChange = _player.Vel.Y * delta + yAccel;

        _player.MoveY(yPosChange);
        _player.Vel.Y += _gravity * delta;

        _timeSinceJump += delta;
    }

    public override void Update_StateTransitions(float _) {
        if (!Input.IsActionPressed("Jump")) {
            float remainingHeight = Mathf.Abs((_startingPos.Y - _player.WallJumpHeight) - _player.Pos.y);
            float remainingTime = _player.WallJumpTime - _timeSinceJump;
            
            _stateMachine.ChangeState<ShortJump>(dict: new() {
                { "RemainingHeight", remainingHeight },
                { "RemainingTime", remainingTime }
            });
            
            return;
        }

        if (_player.Vel.Y >= 0 || _player.Stamina <= 0 || !_player.OnWall()) {
            _stateMachine.ChangeState<Falling>();
            return;
        }
    }
}
