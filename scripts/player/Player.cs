using Godot;
using System.Collections.Generic;

public partial class Player : Node2D {
    [ExportGroup("Horizontal Movement")]
    [Export] public float MovementSpeed = 70;
    [Export] public float FramesToMaxSpeed = 5;
    [Export] public float FramesToMaxSpeedInAir = 8;
    [Export] public Curve ReelbackLerpStrength;

    [ExportGroup("Jumping")]
    [Export] public float JumpHeight = 26;
    [Export] public float JumpDistance = 26;
    [Export] public float JumpBufferTime = 0.15f;
    [Export] public float CoyoteTimeInFrames = 5;
    [Export] public int JumpWallClipThreshold = 3;
    public float JumpTime => JumpDistance / MovementSpeed;
    [ExportGroup("Jumping/Short Jumping")]
    [Export] public float ShortJumpRemainingHeightDivisor = 2;
    [Export] public float ShortJumpRemainingTimeDivisor = 2;
    [ExportGroup("Jumping/Ground Bouncing")]
    [Export] public float BounceHeight = 42;
    [Export] public float BounceDistance = 40;
    public float BounceTime => BounceDistance / MovementSpeed;

    [ExportGroup("Falling")]
    [Export] public float FallDistance = 16;
    [Export] public float TerminalVelocity = 100;
    [Export] public float FastFallTerminalVelocity = 140;
    [Export] public float FastFallGravityMultiplier = 1;
    public float FallTime => FallDistance / MovementSpeed;

    [ExportGroup("Wall Jumping")]
    [Export] public float WallJumpDistanceLeniency = 2;
    [Export] public float WallJumpHeight = 26;
    [Export] public float WallJumpDistance = 26;
    [Export] public float WallJumpMovementSpeed = 70;
    [Export] public float WallJumpPushoffDistance = 16;
    public float WallJumpTime => WallJumpDistance / WallJumpMovementSpeed;
    public float WallJumpPushoffTime => WallJumpPushoffDistance / WallJumpMovementSpeed;
    [ExportGroup("Wall Jumping/Neutral Wall Jumping")]
    [Export] public float NeutralWallJumpHeight = 26;
    [Export] public float NeutralWallJumpTime = 0.25f;
    [Export] public float NeutralWallJumpMovementSpeed = 70;
    public float NeutralWallJumpDistance => NeutralWallJumpMovementSpeed * NeutralWallJumpTime;
    [ExportGroup("Wall Jumping/Wall Bouncing")]
    [Export] public float WallBounceDistanceLeniency = 4;
    [Export] public float WallBounceTimeLeniencyInFrames = 5;
    [Export] public float WallbounceMinimumSpeed = 69;
    [Export] public float WallBounceDistance = 42;
    [Export] public float WallBounceMovementSpeed = 80;
    [Export] public float WallBouncePushoffDistance = 12;
    public float WallBounceTime => WallBounceDistance / WallBounceMovementSpeed;
    public float WallBouncePushoffTime => WallBouncePushoffDistance / WallBounceMovementSpeed;

    [ExportGroup("Climbing")]
    [Export] public float TotalStamina = 8;
    [Export] public float ClimbingSpeed = 25;
    [Export] public float GrabbingStaminaCost = 1;
    [Export] public float ClimbingStaminaCost = 1.5f;
    [Export] public float WallGrippingSpeedDivisor = 2.4f;
    [ExportGroup("Climbing/Climb Jumping")]
    [Export] public float ClimbJumpStaminaCost = 2;
    [ExportGroup("Climbing/Hanging")]
    [Export] public float HangingMoveSpeed = 20;
    [Export] public float HangingStaminaCost = 1.25f;
    [Export] public float HangingMoveStaminaCost = 1.5f;

    [ExportGroup("Dashing")]
    [Export] public float DashDistance = 32;
    [Export] public float DashLengthInFrames = 12;
    [Export] public int DashChargeTotal = 2;
    [Export] public int DashWallClipThreshold = 3;
    public float DashSpeed => DashDistance / (DashLengthInFrames / 60);
    [ExportGroup("Dashing/Wall Sliding")]
    [Export] public float WallSlidingGravity = 250;
    [ExportGroup("Dashing/Wavedashing")]
    [Export] public float WavedashSpeedMultiplier = 1.2f;

    [ExportGroup("Other")]
    [Export] public PlayerState SpawnState;

    [ExportGroup("node refs")]
    [Export] public Collider Collider { get; private set; }
    [Export] public StateMachine StateMachine { get; private set; }
    [Export] public InputBuffer InputBuffer { get; private set; }


    public Vector2 SpawnPosition { get; private set; }
    
    public Vector2PP Pos;
    public Vector2 Vel = Vector2.Zero;
    
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



    public bool CheckCollision(Vector2 offset, Vector2? pos = null, string nodesGroup = "Tiles") => 
        CheckCollision(offset, out _, pos, nodesGroup);
    public bool CheckCollision(float x, float y, Vector2? pos = null, string nodesGroup = "Tiles") =>
        CheckCollision(x, y, out _, pos, nodesGroup);
    
    public bool CheckCollision(
        float x, float y, out List<Collider> collidingWith,
        Vector2? pos = null, string nodesGroup = "Tiles"
    ) =>
        CheckCollision(new(x, y), out collidingWith, pos, nodesGroup);

    public bool CheckCollision(
            Vector2 offset,
            out List<Collider> collidingWith,
            Vector2? pos = null,
            string nodesGroup = "Tiles"
    ) {
        Vector2 ogPos = Pos.Vec;
        
        if (pos != null) {
            Pos.Vec = (Vector2) pos;
        }
        Pos.Vec += offset;
        
        bool colliding = false;
        collidingWith = new();

        foreach (Collider tile in NodesCache.Get(nodesGroup, GetTree())) {
            if (Collider.Intersects(tile, 0, 0)) {
                colliding = true;
                collidingWith.Add(tile);
            }
        }

        Pos.Vec = ogPos;
        return colliding;
    }
    
    public bool InTile(string nodesGroup = "Tiles") => CheckCollision(0, 0, nodesGroup: nodesGroup);
    

    
    public bool MoveAndStop(float x, float y) => MoveAndStop(new(x, y));
    public void MoveAndSlide(float x, float y) => MoveAndSlide(new(x, y));
    public bool MoveX(float x) => MoveAndStop(x, 0);
    public bool MoveY(float y) => MoveAndStop(0, y);
    public bool TestMove(Vector2 vec) => TestMove(new List<Vector2>() { vec });
    public bool TestMove(float x, float y) => TestMove(new Vector2(x, y));
    public Vector2 TestMoveDistance(float x, float y) => TestMoveDistance(new(x, y));
    
    public bool MoveAndStop(Vector2 vec, bool affectVelocity = true) {
        Vector2 normalized = vec.Normalized();
        float magnitude = vec.Length();
        int steps = (int) magnitude;
        
        
        for (int i = 1; i <= steps; i++) {
            if (CheckCollision(normalized)) {
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
    
    public bool TestMove(List<Vector2> vecs) {
        Vector2 pos = Pos.Vec;
        
        foreach (Vector2 vec in vecs) {
            Vector2 normalized = vec.Normalized();
            float magnitude = vec.Length();
            int steps = (int) magnitude;
            

            for (int i = 1; i <= steps; i++) {
                if (CheckCollision(normalized, pos: pos)) {
                    return true;
                }
                
                pos += normalized;
            }
            

            Vector2 vecRemainder = vec - (normalized * steps);
            if (vecRemainder != Vector2.Zero && CheckCollision(vecRemainder, pos: pos)) {
                return true;
            }
            
            pos += vecRemainder;
        }
        
        return false;
    }
    
    public Vector2 TestMoveDistance(Vector2 vec) {
        Vector2 pos = Pos.Vec;
        Vector2 distance = Vector2.Zero;
        
        Vector2 normalized = vec.Normalized();
        float magnitude = vec.Length();
        int steps = (int) magnitude;
        

        for (int i = 1; i <= steps; i++) {
            if (CheckCollision(normalized, pos: pos)) {
                return distance;
            }
            
            pos += normalized;
            distance += normalized;
        }
        

        Vector2 vecRemainder = vec - (normalized * steps);
        if (vecRemainder != Vector2.Zero && !CheckCollision(vecRemainder, pos: pos)) {
            distance += vecRemainder;
        }
        
        return distance;
    }
    
    public bool OnGround(float leniencyDistance) => TestMove(Vector2.Down * leniencyDistance) && !InTile();
    public bool OnCeiling(float leniencyDistance) => TestMove(Vector2.Up * leniencyDistance) && !InTile();
    public bool OnLeftWall(float leniencyDistance) => TestMove(Vector2.Left * leniencyDistance) && !InTile();
    public bool OnRightWall(float leniencyDistance) => TestMove(Vector2.Right * leniencyDistance) && !InTile();
    //////////////////////////////////////
    public bool OnGround() => OnGround(1);
    public bool OnCeiling() => OnCeiling(1);
    public bool OnLeftWall() => OnLeftWall(1);
    public bool OnRightWall() => OnRightWall(1);
    
    public bool OnSurface(float leniency) => (OnGround(leniency) || OnCeiling(leniency) || OnWall(leniency)) && !InTile();
    public bool OnWall(float leniency) => OnLeftWall(leniency) || OnRightWall(leniency);
    public bool OnWallSide(float dir, float leniency) {
        if (dir == -1) {
            return OnLeftWall(leniency);
        }
        return OnRightWall(leniency);
    }
    ////////////////////////////////////////
    public bool OnSurface() => OnSurface(1);
    public bool OnWall() => OnWall(1);
    public bool OnWallSide(float dir) => OnWallSide(dir, 1);
    
    public int GetWallNormal() => GetWallNormal(1);
    public int GetWallNormal(float leniency) {
        float leftDistance = TestMoveDistance(-leniency, 0).Abs().X;
        float rightDistance = TestMoveDistance(leniency, 0).Abs().X;
        
        if (leftDistance < rightDistance) {
            return 1;
        } else if (rightDistance < leftDistance) {
            return -1;
        }
        
        return 0;
    }
}
