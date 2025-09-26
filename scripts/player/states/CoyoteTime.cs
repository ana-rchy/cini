using Godot;
using System;

public partial class CoyoteTime : PlayerState {
    Timer _timer;
    

    public override void _Ready() {
        base._Ready();
        
        _timer = new();
        _timer.OneShot = true;
        AddChild(_timer);
    }
    
    public override bool Prereqs() {
        State lastState = _stateMachine.CurrentState;
        return lastState is OnGround;
    }
    
    public override void Enter() {
        _timer.Start(_player.CoyoteTime);
    }
    

    public override void Update(float delta) {
        BasicXMovement(_player.MovementSpeed, _player.FramesToMaxSpeed, delta);
    }
    
    public override void Update_StateTransitions(float _) {
        if (_player.OnGround()) {
            _stateMachine.ChangeState<OnGround>();
            return;
        }

        if (!_player.OnGround() && _timer.IsStopped()) {
            _stateMachine.ChangeState<Falling>();
            return;
        }

        if (Input.IsActionJustPressed("Jump")) {
            _stateMachine.ChangeState<Jump>();
            return;
        }
    }
}
