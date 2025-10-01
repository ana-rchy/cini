using Godot;
using System;
using System.Diagnostics;

// fsr sometimes when this script is a tool, it just spams out a bunch of errors in the editor
// so i stopped it from doing that when the editor bounding boxes are not needed
// 
// it could be when it tries to print about smth that isnt available in editor
// [Tool]
public partial class PixelCamera : Camera2D {
    [Export] Vector2I _innerBoundsWH = new(24, 24); // width, height
    [Export] Vector2I _outerBoundsWH = new(128, 96);
    [Export] Color _innerBoundsColor = new(0, 1, 0, 0.8f);
    [Export] Color _outerBoundsColor = new(1, 0, 0, 0.8f);
    [Export] public Curve CameraSpeedCurve { get; private set; }
    [Export] public Curve CameraRoomTransitionCurve { get; private set; }
    
    [ExportGroup("node refs")]
    [Export] public Collider PlayerCollider { get; private set; }
    
    // top-left, bottom-right coords
    public Vector2I OuterBoundsTL => PosCentre.VecInt - (_outerBoundsWH / 2);
    public Vector2I OuterBoundsBR => OuterBoundsTL + _outerBoundsWH;
    ////////////////////////////////////////////////////////////////
    public Vector2I InnerBoundsTL => PosCentre.VecInt - (_innerBoundsWH / 2);
    public Vector2I InnerBoundsBR => InnerBoundsTL + _innerBoundsWH;
    
    
    public Room CurrentRoom = null;
    
    public Vector2I Size => (Vector2I) (GetViewport().GetVisibleRect().Size / Zoom);
    
    public Vector2PP PosCentre;
    public Vector2 PosTopLeft {
        get { return PosCentre.Vec - (Size / 2); }
        set { PosCentre.Vec = value + (Size / 2); }
    }
    public Vector2 PosBottomRight {
        get { return PosCentre.Vec + (Size / 2); }
        set { PosCentre.Vec = value - (Size / 2); }
    }
    
    
    
    public override void _Ready() {
        AnchorMode = AnchorModeEnum.DragCenter;
        PosCentre = new(this);
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
