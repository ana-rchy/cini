using System.Collections.Generic;
using Godot;

public partial class WallDashSlide : PlayerState {
    float _gravity;
    
    
    public override bool Prereqs() =>
        _player.OnWall()
        && Input.GetAxis("Left", "Right") == -_player.GetWallNormal()
        && _player.Vel.Y < 0;
    
    public override void Enter(Dictionary<string, object> dict = null) {
        _gravity = _player.WallSlidingGravity;
    }
    
    
    public override void Update(float delta) {
        float yAccel = GetVerletAccelTerm(_gravity, delta);
        float yPosChange = _player.Vel.Y * delta + yAccel;
        
        _player.MoveY(yPosChange);
        _player.Vel.Y += _gravity * delta;
    }
    
    public override void Update_StateTransitions(float _) {
        float xInput = Input.GetAxis("Left", "Right");
        
        if (xInput != -_player.GetWallNormal() || _player.Vel.Y >= 0) {
            _stateMachine.ChangeState<Falling>();
            return;
        }
        
        if (_inputBuffer.ShouldJump()) {
            if (_player.Vel.Abs().Y > _player.WallbounceMinimumSpeed) {
                _stateMachine.ChangeState<WallBounce>();
                return;
            }
                
            if (xInput != 0) {
                _stateMachine.ChangeState<WallJump>();
                return;
            }
            
            if (xInput == 0) {
                _stateMachine.ChangeState<NeutralWallJump>();
                return;
            }
        }
    }
}
