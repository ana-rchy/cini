using Godot;
using System.Collections.Generic;

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
    [Export] public PlayerState SpawnState;
    [Export] public int WallClipThreshold;
    [Export] public int DashClipThreshold;

    [ExportGroup("node refs")]
    [Export] public Collider Collider { get; private set; }
    [Export] public StateMachine StateMachine { get; private set; }
    [Export] public InputBuffer InputBuffer { get; private set; }


    public Vector2 SpawnPosition { get; private set; }
    
    public Vector2PP Pos;
    public Vector2 Vel = Vector2.Zero;
    
    public float TimeSinceJump = 0;
    public float Stamina;
    public int DashCharges = 0;


    public override void _Ready() {
        Vector2 editorPosition = Position;
        SpawnPosition = editorPosition;
        
        // move player to where they were placed in the scene
        Pos = new(this, SpawnPosition);
    }



    public bool CheckCollision(Vector2 offset, string nodesGroup = "Tiles") => CheckCollision(offset, out _, nodesGroup);
    public bool CheckCollision(float x, float y, string nodesGroup = "Tiles") => CheckCollision(x, y, out _, nodesGroup);
    public bool CheckCollision(float x, float y, out List<Collider> collidingWith, string nodesGroup = "Tiles") =>
        CheckCollision(new(x, y), out collidingWith, nodesGroup);

    public bool CheckCollision(Vector2 offset, out List<Collider> collidingWith, string nodesGroup = "Tiles") {
        Vector2 oldPos = Pos.v;
        Pos.v += offset;
        
        bool colliding = false;
        collidingWith = new();

        foreach (Collider tile in GetTree().GetNodesInGroup(nodesGroup)) {
            if (Collider.Intersects(tile, 0, 0)) {
                colliding = true;
                collidingWith.Add(tile);
            }
        }

        Pos.v = oldPos;
        return colliding;
    }
    
    public bool TestXThenYCollision(int offsetX, int offsetY) {
        float ogX = Pos.x;
        float ogY = Pos.y;

        if (MoveX(offsetX)) {
            return true;
        }

        bool result = CheckCollision(0, offsetY);

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

        bool result = CheckCollision(offsetX, 0);

        Pos.x = ogX;
        Pos.y = ogY;

        return result;
    }


    public bool InTile(string nodesGroup = "Tiles") => CheckCollision(0, 0, nodesGroup);
    public bool OnGround() => CheckCollision(0, 1) && !InTile();
    public bool OnCeiling() => CheckCollision(0, -1) && !InTile();
    public bool OnLeftWall() => CheckCollision(-1, 0) && !InTile();
    public bool OnRightWall() => CheckCollision(1, 0) && !InTile();
    public bool OnWall() => OnLeftWall() || OnRightWall();
    public bool OnSurface() => (OnGround() || OnCeiling() || OnWall()) && !InTile();
    public bool InAir() => !OnGround() && !OnCeiling() && !OnWall() && !InTile();

    public int GetWallNormal() {
        int normal = 0;
        normal += OnLeftWall() ? 1 : 0;
        normal += OnRightWall() ? -1 : 0;
        
        return normal;
    }



    
    public bool MoveAndStop(float x, float y) => MoveAndStop(new(x, y));
    public void MoveAndSlide(float x, float y) => MoveAndSlide(new(x, y));
    public bool MoveX(float x) => MoveAndStop(x, 0);
    public bool MoveY(float y) => MoveAndStop(0, y);
    
    public bool MoveAndStop(Vector2 vec) {
        Vector2 normalized = vec.Normalized();
        float magnitude = vec.Length();
        int steps = (int) magnitude;

        for (int i = 1; i <= steps; i++) {
            if (CheckCollision(normalized * i)) {
                return true;
            }

            Pos.v += normalized;
        }


        Vector2 vecRemainder = vec - (normalized * steps);

        if (CheckCollision(vecRemainder)) {
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
            if (!CheckCollision(normalized.X, 0)) {
                Pos.x += normalized.X;
            }

            if (!CheckCollision(0, normalized.Y)) {
                Pos.y += normalized.Y;
            }
        }


        Vector2 vecRemainder = vec - (normalized * steps);

        if (!CheckCollision(vecRemainder.X, 0)) {
            Pos.x += vecRemainder.X;
        }

        if (!CheckCollision(0, vecRemainder.Y)) {
            Pos.y += vecRemainder.Y;
        }
    }
}
