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
    [ExportGroup("Jumping/Short Jumping")]
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
    [ExportGroup("Wall Jumping/Neutral Wall Jumping")]
    [Export] public float NeutralWallJumpHeight;
    [Export] public float NeutralWallJumpRiseDistance;
    [Export] public float NeutralWallJumpPushoffSpeed;

    [ExportGroup("Climbing")]
    [Export] public float TotalStamina;
    [Export] public float ClimbingSpeed;
    [Export] public float GrabbingStaminaCost;
    [Export] public float ClimbingStaminaCost;
    [Export] public float WallSlidingDivisor;
    [ExportGroup("Climbing/Hanging")]
    [Export] public float HangingMoveSpeed;
    [Export] public float HangingStaminaCost;
    [Export] public float HangingMoveStaminaCost;

    [ExportGroup("Dashing")]
    [Export] public float DashDistance;
    [Export] public float DashLength;
    [Export] public int DashChargeTotal;
    public float DashSpeed => DashDistance / DashLength;

    [ExportGroup("Other")]
    [Export] public int WallClipThreshold;
    [Export] public int DashClipThreshold;

    [ExportGroup("node refs")]
    [Export] public InputBuffer InputBuffer { get; private set; }
    [Export] Collider _collider;


    public float TimeSinceJump = 0;

    public Vector2PP Pos;
    public Vector2 Vel = Vector2.Zero;
    public float Stamina;
    public int DashCharges = 0;


    public override void _Ready() {
        // move player to where they were placed in the scene
        float posX = Position.X;
        float posY = Position.Y;

        Pos = new(this);

        Pos.x = posX;
        Pos.y = posY;

        Stamina = TotalStamina;
    }



    public bool CheckFloatCollision(Vector2 offset) => CheckFloatCollision(offset, out _);

    public bool CheckFloatCollision(Vector2 offset, out Collider collidingWith) {
        Vector2 oldPos = Pos.v;
        bool colliding = false;

        Pos.v += offset;

        foreach (Collider tile in GetTree().GetNodesInGroup("Tiles")) {
            if (_collider.Intersects(tile, 0, 0)) {
                colliding = true;
            }
        }

        Pos.v = oldPos;

        collidingWith = null;
        return colliding;
    }

    public bool CheckIntCollision(int offsetX, int offsetY) => CheckIntCollision(offsetX, offsetY, out _);

    public bool CheckIntCollision(int offsetX, int offsetY, out Collider collidingWith) {
        foreach (Collider tile in GetTree().GetNodesInGroup("Tiles")) {
            if (_collider.Intersects(tile, offsetX, offsetY)) {
                collidingWith = tile;
                return true;
            }
        }

        collidingWith = null;
        return false;
    }

    public bool TestXThenYCollision(int offsetX, int offsetY) {
        float ogX = Pos.x;
        float ogY = Pos.y;

        if (MoveX(offsetX)) {
            return true;
        }

        bool result = CheckIntCollision(0, offsetY);

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

        bool result = CheckIntCollision(offsetX, 0);

        Pos.x = ogX;
        Pos.y = ogY;

        return result;
    }


    public bool InTile() => CheckIntCollision(0, 0);
    public bool OnGround() => CheckIntCollision(0, 1) && !InTile();
    public bool OnCeiling() => CheckIntCollision(0, -1) && !InTile();
    public bool OnLeftWall() => CheckIntCollision(-1, 0) && !InTile();
    public bool OnRightWall() => CheckIntCollision(1, 0) && !InTile();
    public bool OnWall() => OnLeftWall() || OnRightWall();
    public bool OnSurface() => (OnGround() || OnCeiling() || OnWall()) && !InTile();
    public bool InAir() => !OnGround() && !OnCeiling() && !OnWall() && !InTile();

    public int GetWallNormal() {
        if (OnLeftWall()) {
            return 1;
        } else if (OnRightWall()) {
            return -1;
        }

        return 0;
    }



    public bool MoveAndStop(Vector2 vec) {
        Vector2 normalized = vec.Normalized();
        float magnitude = vec.Length();
        int steps = (int) magnitude;

        for (int i = 1; i <= steps; i++) {
            if (CheckFloatCollision(normalized * i)) {
                return true;
            }

            Pos.v += normalized;
        }


        Vector2 vecRemainder = vec - (normalized * steps);

        if (CheckFloatCollision(vecRemainder)) {
            return true;
        }

        Pos.v += vecRemainder;

        return false;
    }

    public void MoveAndSlide(Vector2 vec) {
        Vector2 normalized = vec.Normalized();
        float magnitude = vec.Length();
        int steps = (int) magnitude;

        for (int i = 1; i <= steps; i++) {
            if (!CheckFloatCollision(new(normalized.X, 0))) {
                Pos.x += normalized.X;
            }

            if (!CheckFloatCollision(new(0, normalized.Y))) {
                Pos.y += normalized.Y;
            }
        }


        Vector2 vecRemainder = vec - (normalized * steps);

        if (!CheckFloatCollision(new(vecRemainder.X, 0))) {
            Pos.x += vecRemainder.X;
        }

        if (!CheckFloatCollision(new(0, vecRemainder.Y))) {
            Pos.y += vecRemainder.Y;
        }
    }

    public bool MoveX(float distance) {
        float targetPos = Pos.x + distance;

        int lastPos = Pos.xInt;
        int nextPos = (int) targetPos;

        int steps = nextPos - lastPos;
        int dir = Mathf.Sign(steps);

        for (int i = 0; i < Mathf.Abs(steps); i++) {
            if (CheckIntCollision(dir, 0)) {
                return true;
            }

            Pos.x += dir;
        }

        // BUG: e.g. if x-pos is 1.8 and you wanna add on 1.5, it will check for collision for 2.8, but not for 3.3
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
            if (CheckIntCollision(0, dir)) {
                return true;
            }

            Pos.y += dir;
        }

        Pos.y = targetPos;
        return false;
    }
}
