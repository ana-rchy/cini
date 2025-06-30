using Godot;
using System.Diagnostics;

public partial class PlayerStateOld : State {
    protected PlayerOld _player;

    public override void _Ready() {
        base._Ready();

        Debug.Assert(GetNode("../..") is PlayerOld, $"State {Name}'s grandparent is not a Player");

        _player = GetNode<PlayerOld>("../..");
    }
}
