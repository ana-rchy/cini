using Godot;

public partial class PuppetPlayer : Node2D {
    public void MoveTo(int x, int y) {
        Position = new(x, y);
    }
}
