using Godot;
using System;

public partial class Player : Node2D {
    [ExportGroup("Horizontal Movement")]
    [Export] public float MovementSpeed;
    [Export] public float FramesToMaxSpeed;
    [Export] public float FramesToMaxSpeedInAir;

    [ExportGroup("Jumping")]
    [Export] public float JumpHeight;
    [Export] public float JumpRiseDistance;
    [Export] public float JumpFallDistance;
    [Export] public float JumpBufferTime;
    [Export] public float CoyoteTime;

    [ExportGroup("Short Jumping")]
    [Export] public float ShortJumpHeight;
    [Export] public float ShortJumpRiseDistance;

    [ExportGroup("Falling")]
    [Export] public float TerminalVelocity;
    [Export] public float FastFallTerminalVelocity;
    [Export] public float FastFallGravityMultiplier;

    [ExportGroup("Wall Jumping")]
    [Export] public float WallJumpHeight;
    [Export] public float WallJumpRiseDistance;
    [Export] public float WallJumpPushoffSpeed;
    [Export] public float WallJumpPushoffTime;

    [ExportGroup("Neutral Wall Jumping")]
    [Export] public float NeutralWallJumpHeight;
    [Export] public float NeutralWallJumpRiseDistance;
    [Export] public float NeutralWallJumpPushoffSpeed;

    [ExportGroup("Climbing")]
    [Export] public float TotalStamina;
    [Export] public float ClimbingSpeed;
    [Export] public float GrabbingStaminaCost;
    [Export] public float ClimbingStaminaCost;
    [ExportGroup("Climbing/Hanging")]
    [Export] public float HangingMoveSpeed;
    [Export] public float HangingStaminaCost;
    [Export] public float HangingMoveStaminaCost;

    [ExportGroup("Other")]
    [Export] public int WallClipThreshold;
    [Export] public float WallSlidingDivisor;

    [ExportGroup("node refs")]
    [Export] public InputBuffer InputBuffer { get; private set; }
    [Export] Collider _collider;


    public float TimeSinceJump = 0;

    public Vector2PP Pos;
    public Vector2 Vel = Vector2.Zero;
    public float Stamina;


    public override void _Ready() {
        // move player to where they were placed in the scene
        float posX = Position.X;
        float posY = Position.Y;

        Pos = new(this);

        Pos.x = posX;
        Pos.y = posY;

        Stamina = TotalStamina;
    }



    bool TestTileCollision(int offsetX, int offsetY) {
        foreach (Collider tile in GetTree().GetNodesInGroup("Tiles")) {
            if (_collider.Intersects(tile, offsetX, offsetY)) {
                return true;
            }
        }

        return false;
    }

    public bool TestXThenYCollision(int offsetX, int offsetY) {
        float ogX = Pos.x;
        float ogY = Pos.y;

        if (MoveX(offsetX)) {
            return true;
        }

        bool result = TestTileCollision(0, offsetY);

        Pos.x = ogX;
        Pos.y = ogY;

        return result;
    }

    public bool TestYThenXCollision(int offsetY, int offsetX) {
        float ogX = Pos.x;
        float ogY = Pos.y;

        if (MoveY(offsetY)) {
            return true;
        }

        bool result = TestTileCollision(offsetX, 0);

        Pos.x = ogX;
        Pos.y = ogY;

        return result;
    }

    public bool InTile() => TestTileCollision(0, 0);
    public bool OnGround() => TestTileCollision(0, 1) && !InTile();
    public bool OnCeiling() => TestTileCollision(0, -1) && !InTile();
    public bool OnLeftWall() => TestTileCollision(-1, 0) && !InTile();
    public bool OnRightWall() => TestTileCollision(1, 0) && !InTile();
    public bool OnWall() => OnLeftWall() || OnRightWall();

    public int GetWallNormal() {
        if (OnLeftWall()) {
            return 1;
        } else if (OnRightWall()) {
            return -1;
        }

        return 0;
    }



    public bool MoveX(float distance) {
        float targetPos = Pos.x + distance;

        int lastPos = Pos.xInt;
        int nextPos = (int) targetPos;

        int steps = nextPos - lastPos;
        int dir = Mathf.Sign(steps);

        for (int i = 0; i < Mathf.Abs(steps); i++) {
            if (TestTileCollision(dir, 0)) {
                return true;
            }

            Pos.x += dir;
        }

        Pos.x = targetPos;
        return false;
    }

    public bool MoveY(float distance) {
        float targetPos = Pos.y + distance;

        int lastPos = Pos.yInt;
        int nextPos = (int) targetPos;

        int steps = nextPos - lastPos;
        int dir = Mathf.Sign(steps);

        for (int i = 0; i < Mathf.Abs(steps); i++) {
            if (TestTileCollision(0, dir)) {
                return true;
            }

            Pos.y += dir;
        }

        Pos.y = targetPos;
        return false;
    }
}
