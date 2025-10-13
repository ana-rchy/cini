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
        _stateChangesExecuted.Enqueue($"{CurrentState.GetType()}");
        
        CurrentState.Init();
        CurrentState.Enter();
    }

    public override void _PhysicsProcess(double delta) {
        CurrentState.Update((float) delta);
        CurrentState.Update_StateTransitions((float) delta);
        
        // flushing all the state change log msgs after exiting a 'parent' ChangeState()
        string logMsg = "";
        while (_stateChangesExecuted.Count > 0) {
            logMsg += $"{_stateChangesExecuted.Dequeue()} -> ";
        }
        
        if (logMsg != "") {
            Logging.Log(logMsg, Logging.LogCategory.PlayerStateTransitions);
        }
    }

    public void ChangeState<T>(Dictionary<string, object> dict = null, bool forceReEnter = false)
        => ChangeState(typeof(T).ToString(), dict, forceReEnter);
    
    public void ChangeState(string newStateName, Dictionary<string, object> dict = null, bool forceReEnter = false) {
        Logging.Assert(HasNode(newStateName), $"'{GetParent().Name}/{Name}' doesn't have state '{newStateName}'");
        
        if (!forceReEnter && $"{CurrentState.GetType()}" == newStateName) {
            return;
        }
        
        var newState = GetNode<State>(newStateName);
        if (!newState.Prereqs()) {
            string currentStateName = $"{CurrentState.GetType()}";
            
            Logging.Log($"'{currentStateName} -> {newStateName}' didn't meet prerequisites", Logging.LogCategory.PlayerStatePrereqFails);
            
            return;
        }
        
        _stateChangesExecuted.Enqueue(newStateName);
        CurrentState.Exit();
        
        CurrentState = newState;
        if (!CurrentState.EarlyExitStateTransitions()) {
            CurrentState.Enter(dict);
        }
    }
}
