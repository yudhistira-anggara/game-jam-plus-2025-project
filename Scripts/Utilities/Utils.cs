using Godot;
using System;

namespace GameJam
{
    [Tool]
    public static class Utils
    {
        public static void SaveToFile(string content)
        {
            // using var file = FileAccess.Open("res://", FileAccess.ModeFlags.Write);
            // file.StoreString(content);
        }
    }
}