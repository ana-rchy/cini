using System;
using Godot;

public partial class Climbing : PlayerState {
	public override bool Prereqs() =>
		Input.IsActionPressed("Grab") && _player.OnWall() && _player.Stamina > 0;

	public override void Enter() {
		_player.Vel = Vector2.Zero;
	}
	

	public override void Update(float delta) {
		_player.Vel.Y = Input.GetAxis("Up", "Down") * _player.ClimbingSpeed;
		_player.MoveY(_player.Vel.Y * delta);
		
		float staminaCost = _player.Vel.Y == 0 ? _player.GrabbingStaminaCost : _player.ClimbingStaminaCost;
		_player.Stamina -= staminaCost * delta;
	}
	
	public override void Update_StateTransitions() {
		if (!Input.IsActionPressed("Grab") && _inputBuffer.ShouldJump()) {
			if (Input.GetAxis("Left", "Right") != 0) {
				_stateMachine.ChangeState<WallJump>();
				return;
			} else {
				_stateMachine.ChangeState<NeutralWallJump>();
				return;
			}
		}

		if (Input.IsActionJustPressed("Dash")) {
			_stateMachine.ChangeState<Dash>();
			return;
		}

		if (!Input.IsActionPressed("Grab") || !_player.OnWall() || _player.Stamina <= 0) {
			_stateMachine.ChangeState<Falling>();
			return;
		}
	}
}
