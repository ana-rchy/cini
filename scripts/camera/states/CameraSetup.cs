using System.Collections.Generic;
using Godot;

public partial class CameraSetup : PixelCameraState {
    Vector2 _initialPos;
    Room _initialRoom;

    
    public override async void Init() {
        foreach (Room room in GetTree().GetNodesInGroup("Rooms")) {
            if (room.Collider == null) {
                await ToSignal(room, Room.SignalName.Ready);
            }
            
            if (_playerCollider.Intersects(room.Collider)) {
                _camera.CurrentRoom = room;
                break;
            }
        }

        
        if (_camera.PosCentre == null) {
            await ToSignal(_camera, PixelCamera.SignalName.Ready);
        }
        if (_player.Pos == null) {
            await ToSignal(_player, Player.SignalName.Ready);
        }

        
        _camera.PosCentre.Vec = _player.Pos.Vec;

        KeepCameraInsideRoom(_camera, _camera.CurrentRoom);
        
        // _initialPos = _camera.PosCentre.Vec;
        // _initialRoom = _camera.CurrentRoom;
    }
    

    public override void Enter(Dictionary<string, object> dict = null) {
        // _camera.PosCentre.Vec = _initialPos;
        // _camera.CurrentRoom = _initialRoom;
        
        bool insideBounds =
            _camera.OuterBoundsLeft   <= _playerCollider.Left  &&
            _camera.OuterBoundsTop    <= _playerCollider.Top   &&
            _camera.OuterBoundsRight  >= _playerCollider.Right &&
            _camera.OuterBoundsBottom >= _playerCollider.Bottom;

        if (insideBounds) {
            _stateMachine.ChangeState<CameraEaseToPlayer>();
        } else {
            _stateMachine.ChangeState<CameraKeepPlayerInBounds>();
        }
    }
    

    public override void _PhysicsProcess(double _) {
        if (Input.IsActionJustPressed("Respawn")) {
            _stateMachine.ChangeState<CameraSetup>(forceReEnter: true);
        }
    }
}
