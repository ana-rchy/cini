using System.Collections.Generic;
using System.Linq;
using Godot;

public static class NodesCache {
    static Dictionary<string, Node[]> _cache = new();
    
    public static Node[] Get(string group, SceneTree tree) {
        if (!_cache.ContainsKey(group)) {
            Update(group, tree);
        }
        
        return _cache.GetValueOrDefault(group, new Node[0]);
    }
    
    public static void Update(string group, SceneTree tree) {
        _cache[group] = tree.GetNodesInGroup(group).ToArray();
    }
}
