using Godot;

public partial class PlayerStateMachine : StateMachine {
    public override void _Ready() {} // overrides StateMachine's _Ready
    
    // Enter needs to run after the players _Ready, so the states' _player isnt null
    // uses a signal from the player to this node
    void _OnPlayerReady() {
        CurrentState.Enter();
    }
}
