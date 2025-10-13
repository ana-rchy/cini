using System.Collections.Generic;
using Godot;

public partial class OnGround : PlayerState {
    Timer _coyoteTimer;
    
    
    public override void Init() {
        _coyoteTimer = new();
        _coyoteTimer.OneShot = true;
        AddChild(_coyoteTimer);
    }
    
    
    public override bool Prereqs() =>
        _player.OnGround();

    public override void Enter(Dictionary<string, object> dict = null) {
        if (dict != null && dict.ContainsKey("RemainingCoyoteTime")) {
            _coyoteTimer.Start((float) dict["RemainingCoyoteTime"]);
        }
        
        _player.Vel.Y = 0;
        _player.Stamina = _player.TotalStamina;
        _player.DashCharges = _player.DashChargeTotal;
    }
    

    public override void Update(float delta) {
        BasicXMovement(_player.MovementSpeed, _player.FramesToMaxSpeed, delta);
    }
    
    public override void Update_StateTransitions(float _) {
        if (_player.OnGround()) {
            _coyoteTimer.Start(_player.CoyoteTimeInFrames / 60);
        }
        
        
        if (_inputBuffer.ShouldJump() && (_player.OnGround() || !_coyoteTimer.IsStopped())) {
            _stateMachine.ChangeState<Jump>();
            return;
        }
        
        if (!_player.OnGround() && _coyoteTimer.IsStopped()) {
            _stateMachine.ChangeState<Falling>();
            return;
        }

        if (_player.OnWall() && Input.IsActionPressed("Grab")) {
            _stateMachine.ChangeState<Climbing>();
            return;
        }

        if (Input.IsActionJustPressed("Dash")) {
            _stateMachine.ChangeState<Dash>();
            return;
        }
    }
}
