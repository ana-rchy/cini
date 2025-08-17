using Godot;
using System.Diagnostics;

public partial class State : Node {
    protected StateMachine _stateMachine;

    public override void _Ready() {
        Debug.Assert(GetParent() is StateMachine, $"State {Name}'s parent is not a StateMachine");

        _stateMachine = GetParent<StateMachine>();
    }

    public virtual bool Prereqs() { return true; } // prerequisites to entering the state
    public virtual bool EarlyExitStateTransitions() { return false; } // shortcuts to other states, useful in edge cases
    public virtual void Enter() {} // runs once when changing to the state
    public virtual void Update(float delta) {} // runs every StateMachine's _PhysicsProcess()
    public virtual void Update_StateTransitions() {} // extra region to store conditions for switching states
    public virtual void Exit() {} // runs once after switching out the state
}
