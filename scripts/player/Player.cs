using Godot;
using System.Collections.Generic;

public partial class Player : Node2D {
    [ExportGroup("Horizontal Movement")]
    [Export] public float MovementSpeed;
    [Export] public float FramesToMaxSpeed;
    [Export] public float FramesToMaxSpeedInAir;
    [Export] public Curve ReelbackLerpStrength;

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
    [ExportGroup("Climbing/Climb Jumping")]
    [Export] public float ClimbJumpStaminaCost;
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
        string assertMsg(object obj) => $"'{Name}' is missing '{nameof(obj)}'";
        
        Logging.Assert(SpawnState is PlayerState, assertMsg(SpawnState));
        Logging.Assert(Collider is Collider, assertMsg(Collider));
        Logging.Assert(StateMachine is StateMachine, assertMsg(StateMachine));
        Logging.Assert(InputBuffer is InputBuffer, assertMsg(InputBuffer));
        
        
        Vector2 editorPosition = Position;
        SpawnPosition = editorPosition;
        
        // move player to where they were placed in the scene
        Pos = new(this, SpawnPosition);
    }



    public bool CheckCollision(Vector2 offset, Vector2I? offset2 = null, string nodesGroup = "Tiles") => 
        CheckCollision(offset, out _, offset2, nodesGroup);
    
    public bool CheckCollision(float x, float y, Vector2I? offset2 = null, string nodesGroup = "Tiles") =>
        CheckCollision(x, y, out _, offset2, nodesGroup);
    public bool CheckCollision(float x, float y, out List<Collider> collidingWith, Vector2I? offset2 = null, string nodesGroup = "Tiles") =>
        CheckCollision(new(x, y), out collidingWith, offset2, nodesGroup);

    public bool CheckCollision(
            Vector2 offset,
            out List<Collider> collidingWith,
            Vector2I? offset2 = null,
            string nodesGroup = "Tiles"
    ) {
        if (offset2 == null) {
            offset2 = Vector2I.Zero;
        }
        
        Vector2 oldPos = Pos.Vec;
        Pos.Vec += offset;
        
        bool colliding = false;
        collidingWith = new();

        foreach (Collider tile in GetTree().GetNodesInGroup(nodesGroup)) {
            if (Collider.Intersects(tile, offset2.Value.X, offset2.Value.Y)) {
                colliding = true;
                collidingWith.Add(tile);
            }
        }

        Pos.Vec = oldPos;
        return colliding;
    }
    

    public bool InTile(string nodesGroup = "Tiles") => CheckCollision(0, 0, nodesGroup: nodesGroup);
    
    public bool OnGround(Vector2 offset) => CheckCollision(offset, new(0, 1)) && !InTile();
    public bool OnCeiling(Vector2 offset) => CheckCollision(offset, new(0, -1)) && !InTile();
    public bool OnLeftWall(Vector2 offset) => CheckCollision(offset, new(-1, 0)) && !InTile();
    public bool OnRightWall(Vector2 offset) => CheckCollision(offset, new(1, 0)) && !InTile();
    ///////////////////////////////////////////////////////////////////////////
    public bool OnGround(float x, float y) => OnGround(new(x, y)) && !InTile();
    public bool OnCeiling(float x, float y) => OnCeiling(new(x, y)) && !InTile();
    public bool OnLeftWall(float x, float y) => OnLeftWall(new(x, y)) && !InTile();
    public bool OnRightWall(float x, float y) => OnRightWall(new(x, y)) && !InTile();
    /////////////////////////////////////////
    public bool OnGround() => OnGround(0, 0);
    public bool OnCeiling() => OnCeiling(0, 0);
    public bool OnLeftWall() => OnLeftWall(0, 0);
    public bool OnRightWall() => OnRightWall(0, 0);
    
    public bool OnWall(Vector2 offset) => OnLeftWall(offset) || OnRightWall(offset);
    public bool OnWallSide(float dir, Vector2 offset) { if (dir == -1) return OnLeftWall(offset); return OnRightWall(offset); }
    public bool OnSurface(Vector2 offset) => (OnGround(offset) || OnCeiling(offset) || OnWall(offset)) && !InTile();
    //////////////////////////////////////////////////////////
    public bool OnWall(float x, float y) => OnWall(new(x, y));
    public bool OnWallSide(float dir, float x, float y) => OnWallSide(dir, new(x, y));
    public bool OnSurface(float x, float y) => OnSurface(new(x, y));
    /////////////////////////////////////
    public bool OnWall() => OnWall(0, 0);
    public bool OnWallSide(float dir) => OnWallSide(dir, 0, 0);
    public bool OnSurface() => OnSurface(0, 0);
    

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
    
    public bool MoveAndStop(Vector2 vec, bool affectVelocity = true) {
        Vector2 normalized = vec.Normalized();
        float magnitude = vec.Length();
        int steps = (int) magnitude;

        for (int i = 1; i <= steps; i++) {
            if (CheckCollision(normalized * i)) {
                // only affect x or y velocity, if only those are being changed
                if (affectVelocity && vec.X != 0) {
                    Vel.X = 0;
                }
                if (affectVelocity && vec.Y != 0) {
                    Vel.Y = 0;
                }
                
                return true;
            }

            Pos.Vec += normalized;
        }


        Vector2 vecRemainder = vec - (normalized * steps);

        if (CheckCollision(vecRemainder)) {
            if (affectVelocity && vec.X != 0) {
                Vel.X = 0;
            }
            if (affectVelocity && vec.Y != 0) {
                Vel.Y = 0;
            }
            
            return true;
        }

        Pos.Vec += vecRemainder;

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
