using Godot;
using System.Collections.Generic;

public partial class State : Node {
    protected StateMachine _stateMachine;

    public override void _Ready() {
        Ready += Init;
        
        Node parent = GetParent();
        Logging.Assert(parent is StateMachine, $"'{parent.Name}' is not a '{typeof(StateMachine)}'");

        _stateMachine = (StateMachine) parent;
    }
    

    public virtual void Init() {} // called by Ready signal, so you dont need to do base._Ready() in each state's _Ready
    
    public virtual bool Prereqs() { return true; } // prerequisites to entering the state
    public virtual bool EarlyExitStateTransitions() { return false; } // shortcuts to other states, useful in edge cases
    public virtual void Enter(Dictionary<string, object> dict = null) { Enter(); } // runs once when changing to the state
    public virtual void Enter() {}
        
    public virtual void Update(float delta) {} // runs every StateMachine's _PhysicsProcess()
    public virtual void Update_StateTransitions(float delta) {} // extra region to store conditions for switching states
    
    public virtual void Exit() {} // runs once after switching out the state
}
