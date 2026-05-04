using Godot;
using System.Collections.Generic;

public partial class Respawn : PlayerState {
    public override bool Prereqs() {
        State lastState = _stateMachine.CurrentState;
        return lastState is not Respawn;
    }

    public override void Enter() {
        _player.Pos.Vec = _player.SpawnPosition;
        _player.Vel = Vector2.Zero;

        _player.Stamina = _player.TotalStamina;
        _player.DashCharges = _player.DashChargeTotal;

        _stateMachine.ChangeState(_player.SpawnState.Name);
    }


    // can always transition into this state from these conditions
    public override void _PhysicsProcess(double _) {
        if (Input.IsActionJustPressed("Respawn") || _player.InTile("Spikes")) {
            _stateMachine.ChangeState(this.Name);
        }
    }
}
