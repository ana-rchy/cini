using System;
using Godot;

public partial class PixelCameraState : State {
    protected PixelCamera _camera;
    
    protected Player _player;
    protected Collider _playerCollider;
    
    
    public override void _Ready() {
        base._Ready();
        
        Node grandparent = GetNode("../..");
        Logging.Assert(grandparent is PixelCamera, $"{grandparent.Name} is not a '{typeof(PixelCamera)}'");
        _camera = (PixelCamera) grandparent;
        
        _player = _camera.Player;
        _playerCollider = _camera.PlayerCollider;
    }
    

    protected void KeepCameraInsideRoom(PixelCamera camera, Room room) {
        if (room == null) {
            return;
        }


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
}
