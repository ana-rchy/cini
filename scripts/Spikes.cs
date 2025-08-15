using Godot;
using System.Diagnostics;

public partial class Spikes : Node2D {
    [Export] Collider _collider;
    
    public override void _PhysicsProcess(double _) {
        Player player = (Player) GetTree().GetNodesInGroup("Player")[0];
        
        Debug.Assert(player is Player, "Spikes couldn't get player from 'Player' group");
        
        if (player.Collider.Intersects(_collider)) {
            player.StateMachine.ChangeState("Respawn");
        }
    } 
}
