using System.Collections.Generic;
using Godot;

public partial class ShortJump : PlayerState {
    float _gravity;
    

    public override bool Prereqs() =>
        !Input.IsActionPressed("Jump");
    
    public override void Enter(Dictionary<string, object> dict = null) {
        string dictAssertMsg(string msg) => $"{nameof(ShortJump)} Enter's '{nameof(dict)}' {msg}";
        Logging.Assert(dict != null, dictAssertMsg("cant be null"));
        Logging.Assert(dict.ContainsKey("RemainingHeight"), dictAssertMsg("doesnt contain key 'RemainingHeight'"));
        Logging.Assert(dict.ContainsKey("RemainingTime"), dictAssertMsg("doesnt contain key 'RemainingTime'"));
        
        float remainingHeight = (float) dict["RemainingHeight"];
        float remainingTime = (float) dict["RemainingTime"];
        
        float targetHeight = remainingHeight / _player.ShortJumpRemainingHeightDivisor;
        float targetTime = remainingTime / _player.ShortJumpRemainingTimeDivisor;
        
        
        if (targetHeight < 0.01 || targetTime < 0.01) { // prevent NaN/very large gravity values
            _stateMachine.ChangeState<Falling>();
            return;
        }
        
        _gravity = GetGravityWithTime(targetHeight, targetTime);
    }
    

    public override void Update(float delta) {
        BasicXMovement(_player.MovementSpeed, _player.FramesToMaxSpeedInAir, delta);

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
