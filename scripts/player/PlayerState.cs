using System;
using Godot;
using System.Diagnostics;

public partial class PlayerState : State {
    protected Player _player;
    protected InputBuffer _inputBuffer;
    

    public override void _Ready() {
        base._Ready();

        Debug.Assert(GetNode("../..") is Player, $"State {Name}'s grandparent is not a Player");

        _player = GetNode<Player>("../..");
        _inputBuffer = _player.InputBuffer;
    }
    
    

    public bool HorizontalMovement(float movementSpeed, float framesToMaxSpeed, float delta) =>
        PlayerStateUtils.Physics.HorizontalMovement(movementSpeed, framesToMaxSpeed, delta, _player);
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
