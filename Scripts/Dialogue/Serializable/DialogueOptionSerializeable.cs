using Godot;
using System;
using System.Collections.Generic;

namespace GameJam
{
    [Tool]
    public partial class DialogueOptionSerializable
    {
        public string Next { get; set; }
        public string Script { get; set; }
        public string Text { get; set; }

        public DialogueOptionSerializable()
        {
            Next = "";
            Script = "";
            Text = "";
        }

        public DialogueOptionSerializable(DialogueOption option)
        {
            if (option != null)
            {
                _ = option.Next != null ? Next = option.Next : Next = "";
                _ = option.Script != null ? Script = option.Script.ResourcePath : Script = "";
                _ = option.Text != null ? Text = option.Text : Text = "";
            }
        }
    }
}