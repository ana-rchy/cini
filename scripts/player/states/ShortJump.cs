using System.Collections.Generic;
using Godot;

public partial class ShortJump : PlayerState {
    float _gravity, _movementSpeed;
    

    public override bool Prereqs() =>
        !Input.IsActionPressed("Jump");
    
    public override void Enter(Dictionary<string, object> dict = null) {
        string dictAssertMsg(string msg) => $"{nameof(ShortJump)} Enter's '{nameof(dict)}' {msg}";
        Logging.Assert(dict != null, dictAssertMsg("cant be null"));
        Logging.Assert(dict.ContainsKey("RemainingHeight"), dictAssertMsg("doesnt contain key 'RemainingHeight'"));
        Logging.Assert(dict.ContainsKey("RemainingDistance"), dictAssertMsg("doesnt contain key 'RemainingDistance'"));
        
        float remainingHeight = (float) dict["RemainingHeight"];
        float remainingDistance = (float) dict["RemainingDistance"];
        _movementSpeed = (float) dict.GetValueOrDefault("MovementSpeed", _player.MovementSpeed);
        
        float targetHeight = remainingHeight / _player.ShortJumpRemainingHeightDivisor;
        float targetDistance = remainingDistance / _player.ShortJumpRemainingDistanceDivisor;
        
        _gravity = GetGravityWithDistance(targetHeight, targetDistance, _movementSpeed);
    }
    

    public override void Update(float delta) {
        BasicXMovement(_movementSpeed, _player.FramesToMaxSpeedInAir, delta);

        float yAccel = GetVerletAccelTerm(_gravity, delta);
        float yPosChange = _player.Vel.Y * delta + yAccel;

        Clip(0, yPosChange, _player.JumpWallClipThreshold);
        
        _player.MoveY(yPosChange);
        _player.Vel.Y += _gravity * delta;
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
    }
}
