using Godot;
using System;
using System.Diagnostics;
using System.Collections.Generic;

public partial class Logging : Node {
    public enum LogCategory {
        Asserts,
        PlayerStateTransitions,
        PlayerStatePrereqFails,
        Multiplayer,
    }
    
    // { category, is active }
    [Export] Godot.Collections.Dictionary<LogCategory, bool> _activeLogs = new() {
        { LogCategory.Asserts, false },
        { LogCategory.PlayerStateTransitions, false },
        { LogCategory.PlayerStatePrereqFails, false },
        { LogCategory.Multiplayer, true },
    };
    
    static Dictionary<LogCategory, Queue<String>> _logs = new();
    static Dictionary<LogCategory, FileAccess> _logFiles = new();
    
    static string _logsPath = "user://logs";
    
    
    public override void _Ready() {
        if (OS.HasFeature("editor")) {
            _logsPath = "res://logs";
        }
        
        DirAccess.MakeDirAbsolute(_logsPath);
        
        foreach (LogCategory category in Enum.GetValues(typeof(LogCategory))) {
            _logFiles[category] = FileAccess.Open($"{_logsPath}/{category}.log", FileAccess.ModeFlags.Write);
            
            if (_logFiles[category] == null) {
                GD.PrintErr(FileAccess.GetOpenError());
            }
        }
    }
    
    public override void _PhysicsProcess(double _) {
        foreach (LogCategory category in _logs.Keys) {
            while (_logs[category].Count > 0) {
                string logMsg = $"[{Time.GetTimeStringFromSystem()}] {_logs[category].Dequeue()}";
                
                _logFiles[category].StoreString($"{logMsg}\n");
                
                if (_activeLogs[category]) {
                    GD.Print(logMsg);
                }
            }
        }
    }

    public override void _ExitTree() {
        foreach (FileAccess file in _logFiles.Values) {
            file.Close();
            file.Dispose();
        }
    }
    
    
    public static void Log(string msg, LogCategory category) {
        if (!_logs.ContainsKey(category)) {
            _logs[category] = new();
        }
        
        _logs[category].Enqueue(msg);
    }
    
    public static void Assert(bool condition, string msg) {
        if (!condition) {
            Log(msg, LogCategory.Asserts);
        }
        
        Debug.Assert(condition, msg);
    }
}
