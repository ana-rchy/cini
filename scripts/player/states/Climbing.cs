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
		if (_player.OnCeiling() && Input.GetAxis("Left", "Right") != 0 && Input.IsActionPressed("Grab")) {
			_stateMachine.ChangeState<Hanging>();
			return;
		}
		
		if (_inputBuffer.ShouldJump()) {
			float dir = Input.GetAxis("Left", "Right");
			
			if (dir == _player.GetWallNormal()) {
				_stateMachine.ChangeState<WallJump>();
			} else {
				_stateMachine.ChangeState<ClimbJump>();
			}
			
			return;
		}

		if (Input.IsActionJustPressed("Dash")) {
			_stateMachine.ChangeState<Dash>();
			return;
		}
		
		if (Input.IsActionPressed("Up") && _player.OnWall(Vector2.Down) && !_player.OnWall()) {
			// it goes left or right if theres ground there
			// if you hold the opposite direction, it cancels out the posChange
			// if you hold the same direction, it boosts the posChange (more speed)
			// TODO: think about whether the boosting is a good mechanic
			
			int posChange = (int) Input.GetAxis("Left", "Right");
			if (_player.OnGround(Vector2.Left)) posChange--;
			if (_player.OnGround(Vector2.Right)) posChange++;
			
			_player.MoveX(posChange);
			
			if (_player.OnGround()) {
				_stateMachine.ChangeState<OnGround>();
				return;
			}
		} 

		if (!Input.IsActionPressed("Grab") || !_player.OnWall() || _player.Stamina <= 0) {
			_stateMachine.ChangeState<Falling>();
			return;
		}
	}
}
