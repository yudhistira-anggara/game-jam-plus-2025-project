using Godot;
using System;
using System.IO;

namespace GameJam
{
    [Tool]
    public static class Utils
    {
        public static string LoadFromFile(string path)
        {
            using var file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read);
            string content = file.GetAsText();
            return content;
        }

        public static bool IsValidImageExtension(string path)
        {
            string[] validExtensions = { ".png", ".jpg", ".jpeg", ".bmp", ".tiff" };
            string ext = Path.GetExtension(path).ToLower();
            foreach (var e in validExtensions)
            {
                if (ext == e)
                    return true;
            }
            return false;
        }
    }
}