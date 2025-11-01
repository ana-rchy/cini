using System;
using Godot;

public partial class MoreDashes : Powerup {
    [Export] int _extraDashesCount = 1;
    
    protected override void Init() {
        _player.DashChargeTotal += _extraDashesCount;
    }
}
