using Godot;

public partial class CameraKeepPlayerInBounds : PixelCameraState {
    public override bool Prereqs() =>
        _camera.OuterBoundsLeft   >= _playerCollider.Left  ||
        _camera.OuterBoundsTop    >= _playerCollider.Top   ||
        _camera.OuterBoundsRight  <= _playerCollider.Right ||
        _camera.OuterBoundsBottom <= _playerCollider.Bottom;
    

    public override void Update(float delta) {
        float leftDiff = Mathf.Abs(_playerCollider.Left - _camera.OuterBoundsLeft);
        float rightDiff = Mathf.Abs(_playerCollider.Right - _camera.OuterBoundsRight);
        float upDiff = Mathf.Abs(_playerCollider.Top - _camera.OuterBoundsTop);
        float downDiff = Mathf.Abs(_playerCollider.Bottom - _camera.OuterBoundsBottom);
        
        bool xAffected = false;
        bool yAffected = false;

        
        if (_camera.OuterBoundsLeft >= _playerCollider.Left) {
            _camera.PosCentre.x -= leftDiff;
            xAffected = true;
        }
        
        if (_camera.OuterBoundsRight <= _playerCollider.Right) {
            _camera.PosCentre.x += rightDiff;
            xAffected = true;
        }
        
        if (_camera.OuterBoundsTop >= _playerCollider.Top) {
            _camera.PosCentre.y -= upDiff;
            yAffected = true;
        }
        
        if (_camera.OuterBoundsBottom <= _playerCollider.Bottom) {
            _camera.PosCentre.y += downDiff;
            yAffected = true;
        }


        bool shouldEaseX = !xAffected || _player.Vel.X == 0;
        bool shouldEaseY = !yAffected || _player.Vel.Y == 0;

        CameraEaseToPlayer.EaseToPlayer(
            _camera, _playerCollider, delta,
            affectX: shouldEaseX,
            affectY: shouldEaseY
        );
        

        KeepCameraInsideRoom(_camera, _camera.CurrentRoom);
    }

    public override void Update_StateTransitions(float _) {
        foreach (Room newRoom in GetTree().GetNodesInGroup("Rooms")) {
            if (newRoom != _camera.CurrentRoom && _playerCollider.Intersects(newRoom.Collider)) {
                _stateMachine.ChangeState<CameraInRoomTransition>(new() {
                    { "NextRoom", newRoom },
                });
                return;
            }
        }

        
        if ( !Prereqs() ) {
            _stateMachine.ChangeState<CameraEaseToPlayer>();
            return;
        }
    }
}
