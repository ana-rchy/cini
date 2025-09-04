using System;
using Godot;
using System.Diagnostics;

public partial class PlayerState : State {
    protected Player _player;
    protected InputBuffer _inputBuffer;
    

    public override void _Ready() {
        base._Ready();
        
        Node grandparent = GetNode("../..");
        Logging.Assert(grandparent is Player, $"{grandparent.Name} is not a '{typeof(Player)}'");
        _player = (Player) grandparent;
        
        _inputBuffer = _player.InputBuffer;
    }
    
    

    public bool BasicXMovement(float movementSpeed, float framesToMaxSpeed, float delta) =>
        PlayerStateUtils.Physics.BasicXMovement(movementSpeed, framesToMaxSpeed, delta, _player);
    public float GetJumpVelocity(float movementSpeed, float height, float distance) =>
        PlayerStateUtils.Physics.GetJumpVelocity(movementSpeed, height, distance);
    public float GetGravity(float movementSpeed, float height, float distance) =>
        PlayerStateUtils.Physics.GetGravity(movementSpeed, height, distance);
    public float GetVerletAccelTerm(float accel, float delta) =>
        PlayerStateUtils.Physics.GetVerletAccelTerm(accel, delta);

    public void WallClip() =>
        PlayerStateUtils.QoL.WallClip(_player);
   
    public bool CommonInAirTransitions() =>
        PlayerStateUtils.Transitions.CommonInAirTransitions(_player, _stateMachine, _inputBuffer);
}
