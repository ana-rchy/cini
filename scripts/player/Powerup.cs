using System;
using Godot;

public partial class Powerup : Node {
    [Export] protected bool _active = true;
    
    protected PowerupsManager _powerupsManager;
    
    protected Player _player;
    protected PlayerStateMachine _stateMachine;
    
    public override void _Ready() {
        if (!_active) {
            return;
        }
        
        _powerupsManager = GetParent<PowerupsManager>();
        
        _player = _powerupsManager.Player;
        _stateMachine = _powerupsManager.StateMachine;
        
        Init();
    }
    
    protected virtual void Init() {}
}
