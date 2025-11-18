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

        public static string LoadFromFile(string path)
        {
            using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
            string content = file.GetAsText();
            return content;
        }
    }
}