using System.Collections.Generic;
using Godot;

public partial class PowerupsManager : Node {
    [Export] public Player Player { get; private set; }
    [Export] public PlayerStateMachine StateMachine { get; private set; }
}
