using Godot;
using System;
using System.Collections.Generic;

public partial class Logging : Node {
    public enum LogCategory {
        PlayerStateTransitions,
    }
    
    [Export] Godot.Collections.Dictionary<LogCategory, bool> _activeLogs = new() {
        { LogCategory.PlayerStateTransitions, true },
    };
    
    static Dictionary<LogCategory, Queue<String>> _logs = new();
    static Dictionary<LogCategory, FileAccess> _logFiles = new();
    
    
    public override void _Ready() {
        DirAccess.MakeDirAbsolute("res://logs/");
        
        foreach (LogCategory category in Enum.GetValues(typeof(LogCategory))) {
            _logFiles[category] = FileAccess.Open($"res://logs/{category}.log", FileAccess.ModeFlags.Write);
            
            if (_logFiles[category] == null) {
                GD.PrintErr(FileAccess.GetOpenError());
            }
        }
    }
    
    public override void _PhysicsProcess(double _) {
        foreach (LogCategory category in _logs.Keys) {
            if (!_activeLogs.GetValueOrDefault(category, false)) {
                continue;
            }
            
            while (_logs[category].Count > 0) {
                string logMsg = $"[{Time.GetTimeStringFromSystem()}] {_logs[category].Dequeue()}";
                
                _logFiles[category].StoreString($"{logMsg}\n");
                GD.Print(logMsg);
            }
        }
    }

    public override void _ExitTree() {
        foreach (FileAccess file in _logFiles.Values) {
            file.Close();
            file.Dispose();
        }
    }
    
    
    public static void Log(string msg, LogCategory logCategory) {
        if (!_logs.ContainsKey(logCategory)) {
            _logs[logCategory] = new();
        }
        
        _logs[logCategory].Enqueue(msg);
    }
}
