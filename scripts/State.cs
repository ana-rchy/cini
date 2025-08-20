using Godot;
using System.Diagnostics;

public partial class State : Node {
    protected StateMachine _stateMachine;

    public override void _Ready() {
        Node parent = GetParent();
        Logging.Assert(parent is StateMachine, $"'{parent.Name}' is not a '{typeof(StateMachine)}'");

        _stateMachine = (StateMachine) parent;
    }

    public virtual bool Prereqs() { return true; } // prerequisites to entering the state
    public virtual bool EarlyExitStateTransitions() { return false; } // shortcuts to other states, useful in edge cases
    public virtual void Enter() {} // runs once when changing to the state
    public virtual void Update(float delta) {} // runs every StateMachine's _PhysicsProcess()
    public virtual void Update_StateTransitions() {} // extra region to store conditions for switching states
    public virtual void Exit() {} // runs once after switching out the state
}
