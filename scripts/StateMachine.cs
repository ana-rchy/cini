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

        // if the state changes in a states enter, the call stack will be:
        // ChangeState() > [state].Enter() > ChangeState()
        // 
        // this condition makes sure it only prints state changes that arent exclusively used to transition to another state
        // 
        // TODO: probs make it so it does print *every* state change, but in the right order
        // (without this condition, it will print transitionary states in the reverse order, due to the call stack)
        if (state == CurrentState.Name) {
            GD.Print($"{state}");
        }
    }

}
