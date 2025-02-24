using Godot;
using System.Diagnostics;

public partial class PlayerState : State {
    protected Player _player;

    public override void _Ready() {
        base._Ready();

        Debug.Assert(GetNode("../..") is Player, $"State {Name}'s grandparent is not a Player");

        _player = GetNode<Player>("../..");
    }
}
