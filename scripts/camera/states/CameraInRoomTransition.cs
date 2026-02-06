using System.Collections.Generic;
using Godot;

public partial class CameraInRoomTransition : PixelCameraState {
    Room _nextRoom;
    Vector2 _startPos, _endPos;
    
    Curve _roomTransitionCurve;
    float _transitionTime, _timer = 0;
    
    
    public override void Init() {
        _roomTransitionCurve = _camera.CameraRoomTransitionCurve;
        _transitionTime = _roomTransitionCurve.MaxDomain;
    }
    
    
    public override void Enter(Dictionary<string, object> dict = null) {
        Logging.Assert(dict != null, $"{Name} needs Enter dictionary");


        _nextRoom = (Room) dict["NextRoom"];

        _startPos = _camera.PosCentre.Vec;
        KeepCameraInsideRoom(_camera, _nextRoom);
        _endPos = _camera.PosCentre.Vec;
        
        _camera.PosCentre.Vec = _startPos;
        _timer = 0;
    }
    
    
    public override void Update(float _) {
        float weight = _roomTransitionCurve.Sample(_timer);
        weight = Mathf.Clamp(weight, _roomTransitionCurve.MinValue, _roomTransitionCurve.MaxValue);
        
        _camera.PosCentre.Vec = _startPos.Lerp(_endPos, weight);
    }
    
    public override void Update_StateTransitions(float delta) {
        _timer += delta;
        
        if (_timer >= _transitionTime) {
            _stateMachine.ChangeState<CameraKeepPlayerInBounds>();
            return;
        }
        
        foreach (Room newRoom in GetTree().GetNodesInGroup("Rooms")) {
            if (newRoom != _nextRoom && _camera.PlayerCollider.Intersects(newRoom.Collider)) {
                _stateMachine.ChangeState<CameraInRoomTransition>(
                    new() {
                        { "NextRoom", newRoom }
                    },
                    forceReEnter: true
                );

                return;
            }
        }
    }
    
    
    public override void Exit() {
        _camera.CurrentRoom = _nextRoom;
    }
}
