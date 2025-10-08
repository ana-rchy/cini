using System.Collections.Generic;
using Godot;

public partial class Hanging : PlayerState {
    public override bool Prereqs() =>
        Input.IsActionPressed("Grab") && _player.OnCeiling() && _player.Stamina > 0;

    public override void Enter(Dictionary<string, object> dict = null) {
        _player.Vel = Vector2.Zero;
    }
    

    public override void Update(float delta) {
        _player.Vel.X = Input.GetAxis("Left", "Right") * _player.HangingMoveSpeed;
        _player.MoveX(_player.Vel.X * delta);
        
        float staminaCost = _player.Vel.X == 0 ? _player.HangingStaminaCost : _player.HangingMoveStaminaCost;
        _player.Stamina -= staminaCost * delta;
    }
    
    public override void Update_StateTransitions(float _) {
        if (_player.OnWall() && Input.GetAxis("Up", "Down") != 0 && Input.IsActionPressed("Grab")) {
			_stateMachine.ChangeState<Climbing>();
			return;
		}
        
        if (Input.IsActionJustPressed("Jump")) {
            _player.MoveY(1);
            _stateMachine.ChangeState<Falling>();
            return;
        }

        if (Input.IsActionJustPressed("Dash")) {
            _stateMachine.ChangeState<Dash>();
            return;
        }

        if (!Input.IsActionPressed("Grab") || !_player.OnCeiling() || _player.Stamina <= 0) {
            _stateMachine.ChangeState<Falling>();
            return;
        }
    }
}
