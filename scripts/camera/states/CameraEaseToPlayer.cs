using System.Collections.Generic;
using Godot;

public partial class CameraEaseToPlayer : PixelCameraState {
    public override bool Prereqs() =>
        _camera.OuterBoundsLeft   <= _playerCollider.Left  &&
        _camera.OuterBoundsTop    <= _playerCollider.Top   &&
        _camera.OuterBoundsRight  >= _playerCollider.Right &&
        _camera.OuterBoundsBottom >= _playerCollider.Bottom;
    
    
    // smooth interpolating (determined by curve) when between inner and outer bounds //
    public override void Update(float delta) {
        EaseToPlayer(_camera, _playerCollider, delta);

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
            _stateMachine.ChangeState<CameraKeepPlayerInBounds>();
            return;
        }
    }


    // for easy external access
    public static void EaseToPlayer(
        PixelCamera camera, Collider playerCollider, float delta,
        bool affectX = true,
        bool affectY = true
    ) {
        if (!affectX && !affectY) {
            return;
        }


        float[] weightsLRUD = new float[] { // left, right, up, down
            Mathf.InverseLerp(camera.InnerBoundsTL.X, camera.OuterBoundsTL.X, playerCollider.Left),
            Mathf.InverseLerp(camera.InnerBoundsBR.X, camera.OuterBoundsBR.X, playerCollider.Right),
            Mathf.InverseLerp(camera.InnerBoundsTL.Y, camera.OuterBoundsTL.Y, playerCollider.Top),
            Mathf.InverseLerp(camera.InnerBoundsBR.Y, camera.OuterBoundsBR.Y, playerCollider.Bottom),
        };
        
        for (int i = 0; i < weightsLRUD.Length; i++) {
            weightsLRUD[i] = Mathf.Clamp(weightsLRUD[i], 0f, 1f);
        }

        
        float xWeight = Mathf.Max(weightsLRUD[0], weightsLRUD[1]);
        float yWeight = Mathf.Max(weightsLRUD[2], weightsLRUD[3]);
        
        xWeight = camera.CameraSpeedCurve.Sample(xWeight);
        yWeight = camera.CameraSpeedCurve.Sample(yWeight);
        
        xWeight = Mathf.Clamp(xWeight, camera.CameraSpeedCurve.MinDomain, camera.CameraSpeedCurve.MaxDomain);
        yWeight = Mathf.Clamp(yWeight, camera.CameraSpeedCurve.MinDomain, camera.CameraSpeedCurve.MaxDomain);
        

        if (affectX) {
            camera.PosCentre.x = Mathf.Lerp(camera.PosCentre.x, playerCollider.Centre.X, xWeight * delta);
        };
        if (affectY) {
            camera.PosCentre.y = Mathf.Lerp(camera.PosCentre.y, playerCollider.Centre.Y, yWeight * delta);
        };
    }
}
