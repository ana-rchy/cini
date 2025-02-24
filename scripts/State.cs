using Godot;
using System.Diagnostics;

public partial class State : Node {
    protected StateMachine _stateMachine;

    public override void _Ready() {
        Debug.Assert(GetParent() is StateMachine, $"State {Name}'s parent is not a StateMachine");

        _stateMachine = GetParent<StateMachine>();
    }

    public virtual void Enter() {}
    public virtual void Update() {}
}
