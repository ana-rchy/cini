using Godot;
using System;
using System.Diagnostics;

public partial class Respawn : PlayerState {
	[Export] PixelCamera _camera;
	
	
	public override bool Prereqs() {
		State lastState = _stateMachine.CurrentState;
		return lastState is not Respawn;
	}

	public override void _Ready() {
		base._Ready();

		Debug.Assert(_camera is PixelCamera, $"'{Name}' state doesn't have '{nameof(_camera)}' set");
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
	
	
	// can always transition into this state from these conditions
	public override void _PhysicsProcess(double _) {
		if (Input.IsActionJustPressed("Respawn") || _player.InTile("Spikes")) {
			_stateMachine.ChangeState(this.Name);
		}
	}
}
