using System.Collections.Generic;
using Godot;

public partial class NeutralWallJump : PlayerState {
    Vector2 _startingPos;
    float _gravity, _timeSinceJump;


    public override bool Prereqs() => 
        _inputBuffer.ShouldJump() && Input.GetAxis("Left", "Right") == 0
        && (
            _player.OnWall()
            || (_player.OnWall(_player.WallJumpDistanceLeniency) && _player.GetWallNormal(_player.WallJumpDistanceLeniency) != 0)
        );
    
    public override void Enter(Dictionary<string, object> dict = null) {
        _player.Vel.X = _player.NeutralWallJumpPushoffSpeed * _player.GetWallNormal(_player.WallJumpDistanceLeniency);
        
        _player.Vel.Y = GetJumpVelocityWithTime(_player.NeutralWallJumpHeight, _player.NeutralWallJumpTime);
        _gravity = GetGravityWithTime(_player.NeutralWallJumpHeight, _player.NeutralWallJumpTime);

        _startingPos = _player.Pos.Vec;
        _timeSinceJump = 0;
        
        // prevent jump being activated again in another state
        _inputBuffer.StopJumpBufferTimer();
    }
    

    public override void Update(float delta) {
        BasicXMovement(_player.NeutralWallJumpMovementSpeed, _player.FramesToMaxSpeedInAir, delta);

        float yAccel = GetVerletAccelTerm(_gravity, delta);
        float yPosChange = _player.Vel.Y * delta + yAccel;
        
        Clip(0, yPosChange, _player.JumpWallClipThreshold);

        _player.MoveY(yPosChange);
        _player.Vel.Y += _gravity * delta;

        _timeSinceJump += delta;
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
            float remainingHeight = Mathf.Abs((_startingPos.Y - _player.NeutralWallJumpHeight) - _player.Pos.y);
            float remainingDistance =
                Mathf.Abs(_player.NeutralWallJumpDistance - (_player.NeutralWallJumpMovementSpeed * _timeSinceJump));
            
            _stateMachine.ChangeState<ShortJump>(dict: new() {
                { "RemainingHeight", remainingHeight },
                { "RemainingDistance", remainingDistance },
                { "MovementSpeed", _player.NeutralWallJumpMovementSpeed }
            });
            
            return;
        }

    }
}
