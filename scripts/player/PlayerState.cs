using Godot;
using System.Diagnostics;

public partial class PlayerState : State {
    protected Player _player;

    public override void _Ready() {
        base._Ready();

        Debug.Assert(GetNode("../..") is Player, $"State {Name}'s grandparent is not a Player");

        _player = GetNode<Player>("../..");
    }
    
    public void CheckWallClip() {
        if (_player.OnCeiling()) {
            for (int i = 1; i <= _player.WallClipThreshold; i++) {
                if (!_player.TestXThenYCollision(-i, -1)) {
                    _player.MoveX(-i);
                    break;
                }

                if (!_player.TestXThenYCollision(i, -1)) {
                    _player.MoveX(i);
                    break;
                }
            }
        }
    }
    
    // state transitions that happen whenever youre in the air
    public void CommonInAirTransitions() {
        if (_player.OnGround()) {
            _stateMachine.ChangeState("OnGround");
            return;
        }
        
        if (_player.OnWall() && Input.IsActionJustPressed("Jump") && Input.GetAxis("Left", "Right") != 0) {
            _stateMachine.ChangeState("WallJump");
            return;
        }
        
        if (_player.OnWall() && Input.IsActionJustPressed("Jump") && Input.GetAxis("Left", "Right") == 0) {
            _stateMachine.ChangeState("NeutralWallJump");
            return;
        }
    }
}
