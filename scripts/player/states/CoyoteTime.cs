using Godot;
using System;

public partial class CoyoteTime : PlayerState {
    [Export] Timer _timer;
    

    public override void _Ready() {
        base._Ready();
        
        Logging.Assert(_timer is Timer, $"'{GetParent().Name}/{Name}' is missing '{nameof(_timer)}'");
    }
    
    public override bool Prereqs() {
        State lastState = _stateMachine.CurrentState;
        return lastState is OnGround;
    }
    
    public override void Enter() {
        _timer.OneShot = true;
        _timer.Start(_player.CoyoteTime);
    }
    

    public override void Update(float delta) {
        HorizontalMovement(_player.MovementSpeed, _player.FramesToMaxSpeed, delta);
    }
    
    public override void Update_StateTransitions() {
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
