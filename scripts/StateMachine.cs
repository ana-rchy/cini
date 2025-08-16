using Godot;
using System;
using System.Diagnostics;
using System.Collections.Generic;

public partial class StateMachine : Node {
    [Export] public State CurrentState { get; protected set; }

    // needed in cases where ChangeState() is called from within a ChangeState()'s call stack,
    // so that it preserves the correct order of the state changes
    protected Queue<string> _stateChangesExecuted = new();
    

    public override void _Ready() {
        CurrentState.Enter();
    }

    public override void _PhysicsProcess(double delta) {
        // flushing all the state change log msgs after exiting a 'parent' ChangeState()
        while (_stateChangesExecuted.Count > 0) {
            GD.Print($"{_stateChangesExecuted.Dequeue()}");
        }

        CurrentState.Update((float) delta);
        CurrentState.Update_StateTransitions();
    }

    public void ChangeState<T>(bool forceReEnter = false) => ChangeState(typeof(T).ToString(), forceReEnter);
    
    public void ChangeState(string stateName, bool forceReEnter = false) {
        Debug.Assert(HasNode(stateName), $"StateMachine in '{GetParent().Name}' doesn't contain state '{stateName}'");
        
        if (!forceReEnter && $"{CurrentState.GetType()}" == stateName) {
            return;
        }
        
        var state = GetNode<PlayerState>(stateName);
        if (!state.Prereqs()) {
            GD.Print($"'{stateName}' didn't meet prereqs");
            return;
        }
        
        _stateChangesExecuted.Enqueue(stateName);
        CurrentState.Exit();
        
        CurrentState = GetNode<PlayerState>(stateName);
        if (!CurrentState.EarlyExitStateTransitions()) {
            CurrentState.Enter();
        }
    }
}
