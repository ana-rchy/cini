using Godot;
using GC = Godot.Collections;
using System;

public partial class TileMapLevelEditor : TileMapLayer {
    public override void _Ready() {
        foreach (Vector2I tilePos in GetUsedCells()) {
            TileData tileData = GetCellTileData(tilePos);
            
            // TODO: figure out how to fit in an assert here
            PackedScene tileScene = (PackedScene) tileData.GetCustomData("Scene");
            Node2D tile = tileScene.Instantiate<Node2D>();
            tile.Position = tilePos * TileSet.TileSize + (TileSet.TileSize / 2);
            AddChild(tile);
            
            if ((bool) tileData.GetCustomData("SceneHasSprite")) {
                EraseCell(tilePos);
            }
            
            GC.Dictionary additionalData = (GC.Dictionary) tileData.GetCustomData("AdditionalData");
            
            if (additionalData.Count > 0) {
                ((ITile) tile).ModifyTile(additionalData);
            }
        }
        
        // needed so that tiles placed after the player dont get not-added to the cache,
        // which happens cause not all tiles are ready before the player Get's from the NodesCache
        // (i assume _Ready is called in sequence of tile placement)
        NodesCache.Update("Tiles", GetTree());
    }
}
