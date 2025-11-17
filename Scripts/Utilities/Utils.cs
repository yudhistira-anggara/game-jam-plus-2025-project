using Godot;
using System;

namespace GameJam.Utils
{
    public partial class Utils : Node
    {
        public void SaveToFile(string content)
        {
            using var file = FileAccess.Open("res://", FileAccess.ModeFlags.Write);
            file.StoreString(content);
        }
    }
}