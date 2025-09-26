using System;
using Godot;

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
    /////////////////////////////////////////////////////////////////////////////////////////////
    public float GetJumpVelocityWithTime(float height, float time) =>
        PlayerStateUtils.Physics.GetJumpVelocityWithTime(height, time);
    public float GetJumpVelocityWithDistance(float movementSpeed, float height, float distance) =>
        PlayerStateUtils.Physics.GetJumpVelocityWithDistance(movementSpeed, height, distance);
    ////////////////////////////////////////////////////////////
    public float GetGravityWithTime(float height, float time) =>
        PlayerStateUtils.Physics.GetGravityWithTime(height, time);
    public float GetGravityWithDistance(float movementSpeed, float height, float distance) =>
        PlayerStateUtils.Physics.GetGravityWithDistance(movementSpeed, height, distance);
    ////////////////////////////////////////////////////////////
    public float GetVerletAccelTerm(float accel, float delta) =>
        PlayerStateUtils.Physics.GetVerletAccelTerm(accel, delta);

    public void Clip(Vector2 posChange, int clipThreshold) =>
        PlayerStateUtils.QoL.Clip(posChange, clipThreshold, _player);
    public void Clip(float x, float y, int clipThreshold) =>
        PlayerStateUtils.QoL.Clip(new(x, y), clipThreshold, _player);
   
    public bool CommonInAirTransitions() =>
        PlayerStateUtils.Transitions.CommonInAirTransitions(_player, _stateMachine, _inputBuffer);
}
