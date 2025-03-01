using Godot;
using System.Diagnostics;

public partial class StateMachine : Node {
    [Export] public PlayerState CurrentState { get; private set; }

    public override void _Ready() {
        CurrentState.Enter();
    }

    public override void _PhysicsProcess(double delta) {
        CurrentState.Update((float) delta);
    }

    public void ChangeState(string state) {
        Debug.Assert(HasNode(state), $"StateMachine in {GetParent().Name} doesn't contain state \"{state}\"");

        CurrentState = GetNode<PlayerState>(state);
        CurrentState.Enter();

        GD.Print($"{state}");
    }

}
