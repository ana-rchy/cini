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
		float yPosChange = _player.Vel.Y * delta;
		
		// stop player at last wall pixel going up
		while (!_player.OnWall(0, yPosChange) && yPosChange < 0) {
			yPosChange = Mathf.Min(yPosChange + 1, 0);
		}
		
		// stop player at last wall pixel going down
		while (!_player.OnWall(0, yPosChange) && yPosChange > 0) {
			yPosChange = Mathf.Max(yPosChange - 1, 0);
		}
		
		_player.MoveY(yPosChange);
		
		float staminaCost = _player.Vel.Y == 0 ? _player.GrabbingStaminaCost : _player.ClimbingStaminaCost;
		_player.Stamina -= staminaCost * delta;
	}
	
	public override void Update_StateTransitions(float delta) {
		int xDir = (int) Input.GetAxis("Left", "Right");
		int yDir = (int) Input.GetAxis("Up", "Down");
		
		
		if (Input.IsActionJustPressed("Dash")) {
			_stateMachine.ChangeState<Dash>();
			return;
		}
		
		// quick switch to ceiling hanging
		if (_player.OnCeiling() && xDir != 0 && Input.IsActionPressed("Grab")) {
			_stateMachine.ChangeState<Hanging>();
			return;
		}
		
		// wall-based jumps
		if (_inputBuffer.ShouldJump()) {
			if (xDir != 0 && xDir == _player.GetWallNormal()) {
				_stateMachine.ChangeState<WallJump>();
			} else {
				_stateMachine.ChangeState<ClimbJump>();
			}
			
			return;
		}
		
		// ledge climbing + ceiling-ledge unclimbing (???)
		if (xDir != 0 && yDir != 0 && _player.OnWallSide(xDir) && !_player.OnWallSide(xDir, 0, yDir)) {
			_player.MoveY(yDir);
			_player.MoveX(xDir);
			
			if (yDir == -1) {
				_stateMachine.ChangeState<OnGround>();
			} else {
				_stateMachine.ChangeState<Hanging>();
			}
			
			return;
		}
		
		if (!Input.IsActionPressed("Grab") || !_player.OnWall() || _player.Stamina <= 0) {
			_stateMachine.ChangeState<Falling>();
			return;
		}
	}
	
	
	// prevents any climbing speed being carried on into other states
	public override void Exit() {
		_player.Vel = Vector2.Zero;
	}
}
