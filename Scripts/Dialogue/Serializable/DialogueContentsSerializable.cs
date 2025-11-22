using Godot;
using System;
using System.Collections.Generic;

namespace GameJam
{
    public partial class DialogueContentsSerializable
    {
        // Talk
        public string Name { get; set; }
        public string Portrait { get; set; }
        public string Voice { get; set; }

        // Selection
        public string Next { get; set; }

        // Shared
        public string Script { get; set; } = "";
        public string Text { get; set; } = "";
    }
}