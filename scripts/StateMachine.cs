using Godot;
using System.Diagnostics;
using System.Collections.Generic;

public partial class StateMachine : Node {
    [Export] public PlayerState CurrentState { get; private set; }
    
    // needed in cases where ChangeState() is called from within a ChangeState()'s call stack,
    // so that it preserves the correct order of the state changes
    Queue<string> _stateChangesExecuted = new();

    public override void _Ready() {
        CurrentState.Enter();
    }

    public override void _PhysicsProcess(double delta) {
        // flushing all the state change log msgs after exiting a 'parent' ChangeState()
        while (_stateChangesExecuted.Count > 0) {
            GD.Print($"{_stateChangesExecuted.Dequeue()}");
        }
        
        CurrentState.Update((float) delta);
    }

    public void ChangeState(string state) {
        Debug.Assert(HasNode(state), $"StateMachine in {GetParent().Name} doesn't contain state \"{state}\"");
        
        _stateChangesExecuted.Enqueue(state);

        CurrentState = GetNode<PlayerState>(state);
        CurrentState.Enter();
    }
    
    
    
    public void InAirCommonStateTransitions(PlayerOld player) {
        if (player.IsOnFloor()) {
            ChangeState("OnGround");
            return;
        }
        
        if (player.IsOnWall() && Input.IsActionJustPressed("Jump") && Input.GetAxis("Left", "Right") != 0) {
            ChangeState("WallJump");
            return;
        }
        
        if (player.IsOnWall() && Input.IsActionJustPressed("Jump") && Input.GetAxis("Left", "Right") == 0) {
            ChangeState("NeutralWallJump");
            return;
        }
    }
}
