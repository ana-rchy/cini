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
    
    

    public float GetMovementXVel(float xVel, float framesToMaxSpeed, float delta) {
        float movementSpeed = _player.MovementSpeed;

        // accel = speed / time
        float horizontalAccel = movementSpeed / (framesToMaxSpeed / 60);
        float dir = Input.GetAxis("Left", "Right");

        // TODO: implement a soft cap instead of hard cap on speed
        if (dir != 0 && Mathf.Abs(xVel) <= movementSpeed) {
            return Mathf.Clamp(xVel + dir * horizontalAccel * delta, -movementSpeed, movementSpeed);
        } else if (xVel < 0) {
            return MathF.Min(xVel + horizontalAccel * delta, 0);
        } else if (xVel > 0) {
            return MathF.Max(xVel - horizontalAccel * delta, 0);
        }

        return 0;
    }
    
    public float GetVerletAccelTerm(float accel, float delta) {
        // velocity verlet equation:
        // pos += vel * dt + (0.5 * accel * dt^2)
        // vel += acc * dt
        return 0.5f * accel * delta * delta;
    }

    public bool HorizontalMovement(float framesToMaxSpeed, float delta) {
        _player.Vel.X = GetMovementXVel(_player.Vel.X, framesToMaxSpeed, delta);
        return _player.MoveX(_player.Vel.X * delta);
    }

    public void WallClip() {
        if (!_player.OnCeiling()) {
            return;
        }

        for (int x = 1; x <= _player.WallClipThreshold; x++) {
            if (!_player.TestXThenYCollision(-x, -1)) {
                _player.MoveX(-x);
                break;
            }

            if (!_player.TestXThenYCollision(x, -1)) {
                _player.MoveX(x);
                break;
            }
        }
    }
    


    // state transitions that happen whenever youre in the air
    public bool CommonInAirTransitions() {
        if (_player.OnGround()) {
            _stateMachine.ChangeState<OnGround>();
            return true;
        }

        if (_player.OnWall() && _inputBuffer.ShouldJump() && Input.GetAxis("Left", "Right") != 0) {
            _stateMachine.ChangeState<WallJump>();
            return true;
        }

        if (_player.OnWall() && _inputBuffer.ShouldJump() && Input.GetAxis("Left", "Right") == 0) {
            _stateMachine.ChangeState<NeutralWallJump>();
            return true;
        }

        if (Input.IsActionJustPressed("Dash")) {
            _stateMachine.ChangeState<Dash>();
            return true;
        }

        if (_player.OnWall() && Input.IsActionPressed("Grab")) {
            _stateMachine.ChangeState<Climbing>();
            return true;
        }

        if (_player.OnCeiling() && Input.IsActionPressed("Grab")) {
            _stateMachine.ChangeState<Hanging>();
            return true;
        }

        return false;
    }
}
