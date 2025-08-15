using Godot;
using System;
using System.Diagnostics;

public partial class Respawn : PlayerState {
	[Export] PixelCamera _camera;

	public override void _Ready() {
		base._Ready();

		Debug.Assert(_camera is PixelCamera, "Respawn state doesn't have '_camera' set");
	}

	public override void Enter() {
		_player.Pos.v = _player.SpawnPosition;
		_player.Vel = Vector2.Zero;

		_player.TimeSinceJump = 0;
		_player.Stamina = _player.TotalStamina;
		_player.DashCharges = _player.DashChargeTotal;

		_camera._Ready();

		_stateMachine.ChangeState(_player.SpawnState.Name);
	}

	public override void _Process(double _) {
		if (Input.IsActionJustPressed("Respawn")) {
			_stateMachine.ChangeState(this.Name);
		}
	}
}
