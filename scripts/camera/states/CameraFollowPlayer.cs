using System;
using Godot;

public partial class CameraFollowPlayer : PixelCameraState {
    Collider _playerCollider;
    
    
    public override void _Ready() {
        base._Ready();
        _playerCollider = _camera.PlayerCollider;
    }
    
    public override async void Enter() {
        if (_playerCollider == null) {
            await ToSignal(_playerCollider, Collider.SignalName.Ready);
        }
        
        foreach (Room room in GetTree().GetNodesInGroup("Rooms")) {
            if (room.Collider == null) {
                await ToSignal(room, Room.SignalName.Ready);
            }
            
            if (_playerCollider.Intersects(room.Collider)) {
                _camera.CurrentRoom = room;
                break;
            }
        }
    }
    
    
    public override void Update(float delta) {
        CameraMovement(delta);
        
        if (_camera.CurrentRoom != null) {
            ClampCameraToRoom(ref _camera, _camera.CurrentRoom);
        }
    }
    
    public override void Update_StateTransitions(float _) {
        foreach (Room newRoom in GetTree().GetNodesInGroup("Rooms")) {
            if (newRoom != _camera.CurrentRoom && _playerCollider.Intersects(newRoom.Collider)) {
                _stateMachine.ChangeState<CameraInRoomTransition>();
                return;
            }
        }
    }
    
    
    public override void _Process(double _) {
        if (Input.IsActionJustPressed("Respawn")) {
            _stateMachine.ChangeState<CameraFollowPlayer>(forceReEnter: true);
        }
    }
    
    
    
    // force camera to stay within room bounds //
    public static void ClampCameraToRoom(ref PixelCamera camera, Room room) {
        camera.PosCentre.x = Mathf.Clamp(
            camera.PosCentre.x,
            room.Collider.Left + (camera.Size.X / 2),
            room.Collider.Right - (camera.Size.X / 2)
        );
        
        // keeps camera at top of room if the room height is minimum
        if (room.Collider.Height <= Room.MinHeight) {
            camera.PosTopLeft = new(camera.PosTopLeft.X, room.Collider.Top);
            return;
        }
        
        camera.PosCentre.y = Mathf.Clamp(
            camera.PosCentre.y,
            room.Collider.Top + (camera.Size.Y / 2),
            room.Collider.Bottom - (camera.Size.Y / 2)
        ); 
    }
    
    void CameraMovement(float delta) {
        // forces camera to move if player moves outside the outer bounds, so the player stays within them //
        //
        // x/yForceMoved is used so camera-player desync doesnt occur -
        // no smoothing if already moving to keep the player in-bounds
        bool xForceMoved, yForceMoved;
        xForceMoved = yForceMoved = false;
        
        float leftDiff = Mathf.Min(_playerCollider.Left - _camera.OuterBoundsTL.X, 0);
        float rightDiff = Mathf.Max(_playerCollider.Right - _camera.OuterBoundsBR.X, 0);
        float upDiff = Mathf.Min(_playerCollider.Top - _camera.OuterBoundsTL.Y, 0);
        float downDiff = Mathf.Max(_playerCollider.Bottom - _camera.OuterBoundsBR.Y, 0);
        
        float xDiff = leftDiff + rightDiff;
        float yDiff = upDiff + downDiff;
        
        if (xDiff != 0) {
            _camera.PosCentre.x += xDiff;
            xForceMoved = true;
        }
        if (yDiff != 0) {
            _camera.PosCentre.y += yDiff;
            yForceMoved = true;
        }
    
        // smooth interpolating (determined by curve) when between inner and outer bounds //
        float[] weightsLRUD = new float[] { // left, right, up, down
            Mathf.InverseLerp(_camera.InnerBoundsTL.X, _camera.OuterBoundsTL.X, _playerCollider.Left),
            Mathf.InverseLerp(_camera.InnerBoundsBR.X, _camera.OuterBoundsBR.X, _playerCollider.Right),
            Mathf.InverseLerp(_camera.InnerBoundsTL.Y, _camera.OuterBoundsTL.Y, _playerCollider.Top),
            Mathf.InverseLerp(_camera.InnerBoundsBR.Y, _camera.OuterBoundsBR.Y, _playerCollider.Bottom),
        };
        
        for (int i = 0; i < weightsLRUD.Length; i++) {
            weightsLRUD[i] = Mathf.Clamp(weightsLRUD[i], 0f, 1f);
        }
        
        float xWeight = Mathf.Max(weightsLRUD[0], weightsLRUD[1]);
        float yWeight = Mathf.Max(weightsLRUD[2], weightsLRUD[3]);
        
        xWeight = _camera.CameraSpeedCurve.Sample(xWeight);
        yWeight = _camera.CameraSpeedCurve.Sample(yWeight);
        
        xWeight = Mathf.Clamp(xWeight, _camera.CameraSpeedCurve.MinDomain, _camera.CameraSpeedCurve.MaxDomain);
        yWeight = Mathf.Clamp(yWeight, _camera.CameraSpeedCurve.MinDomain, _camera.CameraSpeedCurve.MaxDomain);
        
        if (!xForceMoved) {
            _camera.PosCentre.x = Mathf.Lerp(_camera.PosCentre.x, _playerCollider.Centre.X, xWeight * (float) delta);
        }
        if (!yForceMoved) {
            _camera.PosCentre.y = Mathf.Lerp(_camera.PosCentre.y, _playerCollider.Centre.Y, yWeight * (float) delta);
        }
    }
}
