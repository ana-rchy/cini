using Godot;
using System;
using System.Diagnostics;

// fsr sometimes when this script is a tool, it just spams out a bunch of errors in the editor
// so i stopped it from doing that when the editor bounding boxes are not needed
// 
// it could be when it tries to print about smth that isnt available in editor
// [Tool]
public partial class PixelCamera : Camera2D {
    [Export] public Vector2I _innerBoundsWH, _outerBoundsWH; // width, height
    [Export] Color _innerBoundsColor = new(0, 1, 0, 0.8f);
    [Export] Color _outerBoundsColor = new(1, 0, 0, 0.8f);
    [Export] Curve _cameraSpeedCurve;
    
    [ExportGroup("node refs")]
    [Export] Collider _playerCollider;
    
    Vector2PP _pos;
    
    // top-left, bottom-right coords
    Vector2I _innerBoundsTL => _pos.VecInt - (_innerBoundsWH / 2);
    Vector2I _innerBoundsBR => _innerBoundsTL + _innerBoundsWH;
    Vector2I _outerBoundsTL => _pos.VecInt - (_outerBoundsWH / 2);
    Vector2I _outerBoundsBR => _outerBoundsTL + _outerBoundsWH;
    
    
    public override void _Ready() {
        Logging.Assert(_playerCollider is Collider, $"'{GetParent().Name}/{Name}' is missing '{nameof(_playerCollider)}'");
        
        _pos = new(this);
        _pos.Vec = _playerCollider.Centre; 
    }
    
    public override void _Process(double delta) {
        // forces camera to move if player moves outside the outer bounds, so the player stays within them //
        //
        // x/yForceMoved is used so camera-player desync doesnt occur -
        // no smoothing if already moving to keep the player in-bounds
        bool xForceMoved, yForceMoved;
        xForceMoved = yForceMoved = false;
        
        float leftDiff = Mathf.Min(_playerCollider.Left - _outerBoundsTL.X, 0);
        float rightDiff = Mathf.Max(_playerCollider.Right - _outerBoundsBR.X, 0);
        float upDiff = Mathf.Min(_playerCollider.Top - _outerBoundsTL.Y, 0);
        float downDiff = Mathf.Max(_playerCollider.Bottom - _outerBoundsBR.Y, 0);
        
        float xDiff = leftDiff + rightDiff;
        float yDiff = upDiff + downDiff;
        
        if (xDiff != 0) {
            _pos.x += xDiff;
            xForceMoved = true;
        }
        if (yDiff != 0) {
            _pos.y += yDiff;
            yForceMoved = true;
        }
        
        
    
        // smooth interpolating (determined by curve) when between inner and outer bounds //
        float[] weightsLRUD = new float[] { // left, right, up, down
            Mathf.InverseLerp(_innerBoundsTL.X, _outerBoundsTL.X, _playerCollider.Left),
            Mathf.InverseLerp(_innerBoundsBR.X, _outerBoundsBR.X, _playerCollider.Right),
            Mathf.InverseLerp(_innerBoundsTL.Y, _outerBoundsTL.Y, _playerCollider.Top),
            Mathf.InverseLerp(_innerBoundsBR.Y, _outerBoundsBR.Y, _playerCollider.Bottom),
        };
        
        for (int i = 0; i < weightsLRUD.Length; i++) {
            weightsLRUD[i] = Mathf.Clamp(weightsLRUD[i], 0f, 1f);
        }
        
        float xWeight = Mathf.Max(weightsLRUD[0], weightsLRUD[1]);
        float yWeight = Mathf.Max(weightsLRUD[2], weightsLRUD[3]);
        
        xWeight = _cameraSpeedCurve.Sample(xWeight);
        yWeight = _cameraSpeedCurve.Sample(yWeight);
        
        xWeight = Mathf.Clamp(xWeight, _cameraSpeedCurve.MinDomain, _cameraSpeedCurve.MaxDomain);
        yWeight = Mathf.Clamp(yWeight, _cameraSpeedCurve.MinDomain, _cameraSpeedCurve.MaxDomain);
        
        if (!xForceMoved) {
            _pos.x = Mathf.Lerp(_pos.x, _playerCollider.Centre.X, xWeight * (float) delta);
        }
        if (!yForceMoved) {
            _pos.y = Mathf.Lerp(_pos.y, _playerCollider.Centre.Y, yWeight * (float) delta);
        }
    }
    
    
    public override void _Draw() {
        if (Engine.IsEditorHint()) {
            Vector2I innerVec = _innerBoundsWH;
            Rect2 innerRect = new(-innerVec.X / 2, -innerVec.Y / 2, innerVec.X, innerVec.Y);
            DrawRect(innerRect, _innerBoundsColor, filled: false);
            
            Vector2I outerVec = _outerBoundsWH;
            Rect2 outerRect = new(-outerVec.X / 2, -outerVec.Y / 2, outerVec.X, outerVec.Y);
            DrawRect(outerRect, _outerBoundsColor, filled: false);
        }
    }
}
