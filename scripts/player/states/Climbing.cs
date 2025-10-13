using System.Collections.Generic;
using Godot;

public partial class Climbing : PlayerState {
	public override bool Prereqs() =>
		Input.IsActionPressed("Grab") && _player.OnWall() && _player.Stamina > 0;

	public override void Enter(Dictionary<string, object> dict = null) {
		_player.Vel = Vector2.Zero;
	}
	

	public override void Update(float delta) {
		_player.Vel.Y = Input.GetAxis("Up", "Down") * _player.ClimbingSpeed;
		float yPosChange = _player.Vel.Y * delta;
		
		// stop player at last wall pixel going up
		while (
			!_player.TestMove(new List<Vector2>() { new(0, yPosChange), new(-_player.GetWallNormal(), 0) })
			&& yPosChange < 0
		) {
			// need to use min/max so yPosChange doesnt go above/under 0
			yPosChange = Mathf.Min(yPosChange + 1, 0);
		}
		
		// stop player at last wall pixel going down
		while (
			!_player.TestMove(new List<Vector2>() { new(0, yPosChange), new(-_player.GetWallNormal(), 0) })
			&& yPosChange > 0
		) {
			yPosChange = Mathf.Max(yPosChange - 1, 0);
		}
		
		_player.MoveY(yPosChange);
		
		
		float staminaCost = _player.Vel.Y == 0 ? _player.GrabbingStaminaCost : _player.ClimbingStaminaCost;
		_player.Stamina -= staminaCost * delta;
	}
	
	public override void Update_StateTransitions(float delta) {
		int xInput = (int) Input.GetAxis("Left", "Right");
		int yInput = (int) Input.GetAxis("Up", "Down");
		
		
		if (_inputBuffer.ShouldDash()) {
			_stateMachine.ChangeState<Dash>();
			return;
		}
		
		// quick switch to ceiling hanging
		if (_player.OnCeiling() && xInput != 0 && Input.IsActionPressed("Grab")) {
			_stateMachine.ChangeState<Hanging>();
			return;
		}
		
		// wall-based jumps
		if (_inputBuffer.ShouldJump()) {
			if (xInput != 0 && xInput == _player.GetWallNormal()) {
				_stateMachine.ChangeState<WallJump>();
			} else {
				_stateMachine.ChangeState<ClimbJump>();
			}
			
			return;
		}
		
		// ledge climbing + ceiling-ledge unclimbing (???)
		if (
			xInput != 0 && yInput != 0 && _player.OnWallSide(xInput) &&
			!_player.TestMove(new List<Vector2>() { new(0, yInput), new(xInput, 0) })
		) {
			_player.MoveY(yInput);
			_player.MoveX(xInput);
			
			if (yInput == -1) {
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
