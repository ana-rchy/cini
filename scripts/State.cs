using Godot;
using System;

public partial class State : Node {
    protected StateMachine _stateMachine;

    public override void _Ready() {
        _stateMachine = GetParent<StateMachine>();
    }

    public virtual void Enter() {}
    public virtual void Update() {}
}
